using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Helpers;
using BasePackageModule2.Models;
using InstaSharp;
using InstaSharp.Exceptions;
using InstaSharp.Interface;
using InstaSharp.Model;
using InstaSharp.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Order = BasePackageModule2.Models.Order;

namespace BasePackageModule1.Controllers
{
    [Authorize]
    public class BuyNowController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private IInstaMojoConfiguration _instaMojo;
        private IRazorPayConfiguration _razorPay;
        public BuyNowController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IInstaMojoConfiguration instaMojo, IRazorPayConfiguration razorPay)
        {
            _context = context;
            _userManager = userManager;
            _instaMojo = instaMojo;
            _razorPay = razorPay;
        }

        public async Task<IActionResult> Index(int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var addresses = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();
            ViewData["productId"] = productId;
            ViewData["qty"] = qty;

            ViewData["Addresses"] = addresses;

            return View();
        }
        public IActionResult ThankYou()
        {
            return View();
        }
        public async Task<IActionResult> Address(Address address, int productId, int qty)
        {
            if (!ModelState.IsValid)
            {
                ViewData["productId"] = productId;
                ViewData["qty"] = qty;

                return View(nameof(Index), address);
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            address.UserId = user.Id;
            _context.Add(address);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PaymentOptions), new { addressId = address.Id, productId, qty });
        }

        public async Task<IActionResult> CheckOutOptions(int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }

            var cOptions = new ACheckoutoptions
            {
                ProductId = productId,
                Qty = qty
            };

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            var subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;

            return View(cOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutOptions(string PaymentMode, int productId, int qty)
        {
            if (productId == 0 || qty == 0)
            {
                return BadRequest();
            }
            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;


            if (PaymentMode == null)
            {
                ModelState.AddModelError("PaymentMode", "Please Select Payment Mode.");
                var cOptions = new ACheckoutoptions
                {
                    ProductId = productId,
                    Qty = qty
                };

                return View(cOptions);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);



            if (PaymentMode == "payOnline")
            {

            
                try
                {
                    var transactionId = Guid.NewGuid().ToString();

                    #region   1. Create Payment Order

                    RazorpayClient client = new RazorpayClient(_razorPay.KeyID, _razorPay.KeySecret);

                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", subtotal);
                    options.Add("currency", "INR");
                    options.Add("payment_capture", true);
                    Razorpay.Api.Order order = client.Order.Create(options);

                    var orderId = order["id"].ToString();
                    var orderJson = order.Attributes.ToString();
                    return Ok(orderJson);



                   
                    #endregion

                }
                catch (BaseException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return BadRequest();
            }

            if (PaymentMode == "payAtShop")
            {
                var order = new Order
                {
                    UserId = user.Id,
                    AddressId = null,
                    PaymentStatus = "Pay At Shop",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                OrderProduct orderProduct = new OrderProduct
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Quantity = qty
                };
                _context.Add(orderProduct);


                await _context.SaveChangesAsync();


                return View(nameof(ThankYou));
            }

            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> PaymentOptions(int addressId, int productId, int qty)
        {
            if (addressId == 0 || productId == 0 || qty == 0)
            {
                return BadRequest();
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);


            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;

            var transactionId = Guid.NewGuid().ToString();

            RazorpayClient client = new RazorpayClient(_razorPay.KeyID, _razorPay.KeySecret);

            // Generate random receipt number for order
            Random randomObj = new Random();

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", subtotal * 100);  // Amount will in paise
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0");


            Razorpay.Api.Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            //Payment payment = client.Payment.Fetch(orderId);

            //return Ok(payment);

            
            var aPaymentOptions = new APaymentOptions
            {
                AddressId = addressId,
                ProductId = productId,
                Qty = qty,
                orderId = orderId,
                razorpayKey = _razorPay.KeyID,
                amount = subtotal * 100,
                currency = "INR",
                name = user.FullName,
                email = user.Email,
                contactNumber = user.PhoneNumber,
            };

            return View(aPaymentOptions);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentOptions(int AddressId, string PaymentOption, int productId, int qty)
        {
            double subtotal = 0;

            var product = await _context.Products.FindAsync(productId);
            subtotal = (double)(product.FinalPrice * qty);

            ViewData["Items"] = subtotal;

            if (PaymentOption == null || AddressId == 0 || productId == 0 || qty == 0)
            {
                ModelState.AddModelError("PaymentOption", "Please Select Payment option.");

                var aPaymentOptions = new APaymentOptions
                {
                    AddressId = AddressId,
                    ProductId = productId,
                    Qty = qty
                };
                return View(aPaymentOptions);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var add = await _context.Addresses.FirstOrDefaultAsync(i => i.Id == AddressId);

            if (add == null)
            {
                return BadRequest();
            }

            
            if (PaymentOption == "cashOnDelivery")
            {
                var order = new Order
                {
                    UserId = user.Id,
                    AddressId = add.Id,
                    PaymentStatus = "Cash on Delivery",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                OrderProduct orderProduct = new OrderProduct
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Quantity = qty
                };
                _context.Add(orderProduct);

                await _context.SaveChangesAsync();


                return View(nameof(ThankYou));
            }
            return BadRequest();

        }



        [HttpPost]
        public async Task<ActionResult> Complete(string rzp_paymentid, string rzp_orderid, int productId, int qty)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);


            // Payment data comes in url so we have to get it from url

            // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
            string paymentId = rzp_paymentid;

            // This is orderId
            string orderId = rzp_orderid;

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(_razorPay.KeyID, _razorPay.KeySecret);

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];

            var product = await _context.Products.FindAsync(productId);

            var order = new Order
            {
                UserId = user.Id,
                TransactionId = orderId,
                Amount = Double.Parse(amt)/100,
                PaymentId = paymentId,
            };
            if (paymentCaptured.Attributes["status"] == "captured")
            {
                order.PaymentStatus = "Paid";
            }
            else
            {
                order.PaymentStatus = "Failed";
            }

            _context.Add(order);
            await _context.SaveChangesAsync();

            OrderProduct orderProduct = new OrderProduct
            {
                ProductId = product.Id,
                OrderId = order.Id,
                Quantity = qty
            };
            _context.Add(orderProduct);

            await _context.SaveChangesAsync();

            
            if (paymentCaptured.Attributes["status"] == "captured")
            {
                
                return RedirectToAction("ThankYou");
            }
            else
            {
                return RedirectToAction("PaymentFailed");
            }
        }
        public IActionResult PaymentFailed()
        {
            return View();
        }
        public class APaymentOptions
        {
            public string razorpayKey;
            public double amount;
            public string currency;
            public string name;
            public string orderId;
            public string email;
            public string contactNumber;

            public int AddressId { get; set; }
            [Required]
            public string PaymentOption { get; set; }

            public int ProductId { get; set; }
            public int Qty { get; set; }
        }
        public class ACheckoutoptions
        {
            [Required]
            public string PaymentMode { get; set; }

            public int ProductId { get; set; }
            public int Qty { get; set; }
        }

        public class OrderModel
        {
            public string orderId { get; set; }
            public string razorpayKey { get; set; }
            public double amount { get; set; }
            public string currency { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string contactNumber { get; set; }
            public string address { get; set; }
            public string description { get; set; }
        }
    }
}
