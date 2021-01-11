using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
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

namespace BasePackageModule1.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IInstaMojoConfiguration _instaMojo;
        private IRazorPayConfiguration _razorPay;
        

        public CheckoutController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IInstaMojoConfiguration instaMojo, IRazorPayConfiguration razorPay)
        {
            _userManager = userManager;
            _context = context;
            _instaMojo = instaMojo;
            _razorPay = razorPay;
        }


        
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var addresses = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();

            ViewData["Addresses"] = addresses;

            return View();
        }


        public async Task<IActionResult> Address(Address address)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index),address);
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            address.UserId = user.Id;
            _context.Add(address);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PaymentOptions), new { addressId = address.Id });
        }
       

 

        public IActionResult ThankYou()
        {
            return View();
        }

       
        public async Task<IActionResult> CheckOutOptions()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;

            return View();

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutOptions(string PaymentMode)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;


            if (PaymentMode == null)
            {
                ModelState.AddModelError("PaymentMode", "Please Select Payment Mode.");
                return View();
            }

           

            if (PaymentMode == "payAtShop")
            {
                var order = new BasePackageModule2.Models.Order
                {
                    UserId = user.Id,
                    AddressId = null,
                    PaymentStatus = "Pay At Shop",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = cart.Product.Id,
                        OrderId = order.Id,
                        Quantity = cart.Qty
                    };
                    _context.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
                _context.RemoveRange(carts);
                await _context.SaveChangesAsync();

                return View(nameof(ThankYou));
            }

            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> PaymentOptions(int addressId)
        {
            if (addressId == 0)
            {
                return BadRequest();
            }

           
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);


            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) 
                    subtotal += (double)item.Product.FinalPrice * item.Qty;
            }

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

            var aPaymentOptions = new APaymentOptions
            {
                AddressId = addressId,
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
        public async Task<IActionResult> PaymentOptions(int AddressId, string PaymentOption)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            double subtotal = 0;

            foreach (var item in await _context.Carts.Include(a => a.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                if (item.Product.FinalPrice != null) subtotal += (double)item.Product.FinalPrice * item.Qty;
            }
            ViewData["Items"] = subtotal;

            if (PaymentOption == null || AddressId == 0)
            {
                ModelState.AddModelError("PaymentOption", "Please Select Payment option.");
                return View();
            }

            var add = await _context.Addresses.FirstOrDefaultAsync(i => i.Id == AddressId);

            if (add == null)
            {
                return BadRequest();
            }

           


            if (PaymentOption == "payOnline")
            {
                
                if (!await _context.Carts.AnyAsync(u => u.UserId == user.Id))
                {
                    return RedirectToAction("Index", "Cart");
                }

                string instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT, instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;


                if (_instaMojo.ClientId.Contains("test") || _instaMojo.ClientSecret.Contains("test"))
                {
                    instaEndpoint = InstamojoConstants.INSTAMOJO_API_ENDPOINT;
                    instaAuthEndpoint = InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT;
                }

                else
                {
                    instaEndpoint = "https://api.instamojo.com/v2/";
                    instaAuthEndpoint = "https://www.instamojo.com/oauth2/token/";
                }

                IInstamojo objClass;
                try
                {
                    objClass = InstamojoImplementation.getApi(
                        _instaMojo.ClientId,
                        _instaMojo.ClientSecret,
                        instaEndpoint,
                        instaAuthEndpoint);
                }
                catch (Exception e)
                {
                    return Content("Something went wrong, Please try again after sometime");
                }


               
                try
                {
                    var transactionId = Guid.NewGuid().ToString();

                    #region   1. Create Payment Order
                    //  Create Payment Order
                    var objPaymentRequest = new PaymentOrder
                    {
                        name = $"{user.FirstName} {user.LastName}",
                        email = user.Email,
                        phone = add.MobileNumber.Replace("+", ""),
                        amount = subtotal,
                        currency = "INR",
                        description = "",
                        webhook_url = "https://poojacraft.in/Checkout/WebHook",
                        //webhook_url =   Url.Action("WebHook", "Checkout", null, protocol: "https"),
                        transaction_id = transactionId,
                        redirect_url = Url.Action("ProcessPayment", "Checkout", null, protocol: "https")
                    };
                    //Required POST parameters
                    //Extra POST parameters 

                    if (objPaymentRequest.validate())
                    {

                        if (objPaymentRequest.emailInvalid)
                        {
                            return Content("Email is not valid");
                        }
                        if (objPaymentRequest.nameInvalid)
                        {
                            return Content("Name is not valid");
                        }
                        if (objPaymentRequest.phoneInvalid)
                        {
                            return Content("Phone is not valid");
                        }
                        if (objPaymentRequest.amountInvalid)
                        {
                            return Content("Amount is not valid");
                        }
                        if (objPaymentRequest.currencyInvalid)
                        {
                            return Content("Currency is not valid");
                        }
                        if (objPaymentRequest.transactionIdInvalid)
                        {
                            return Content("Transaction Id is not valid");
                        }
                        if (objPaymentRequest.redirectUrlInvalid)
                        {
                            return Content("Redirect Url Id is not valid");
                        }
                        if (objPaymentRequest.webhookUrlInvalid)
                        {
                            return Content("Webhook URL is not valid");
                        }

                    }
                    else
                    {
                        try
                        {
                            var order = new BasePackageModule2.Models.Order
                            {
                                UserId = user.Id,
                                AddressId = add.Id,
                                PaymentStatus = "Pending",
                                TransactionId = transactionId,
                                Amount = subtotal,
                            };
                            _context.Add(order);
                            await _context.SaveChangesAsync();

                            foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                            {
                                OrderProduct orderProduct = new OrderProduct
                                {
                                    ProductId = cart.Product.Id,
                                    OrderId = order.Id,
                                    Quantity = cart.Qty
                                };
                                _context.Add(orderProduct);
                            }

                            await _context.SaveChangesAsync();

                            CreatePaymentOrderResponse objPaymentResponse = objClass.CreateNewPaymentRequest(objPaymentRequest);
                            return Redirect(objPaymentResponse.payment_options.payment_url);

                        }

                        catch (ArgumentNullException ex)
                        {
                            throw ex;
                        }
                        catch (WebException ex)
                        {
                            throw ex;
                        }
                        catch (IOException ex)
                        {
                            throw ex;
                        }
                        catch (InvalidPaymentOrderException ex)
                        {
                            throw ex;
                        }
                        catch (ConnectionException ex)
                        {
                            throw ex;
                        }
                        catch (BaseException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
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

            if (PaymentOption == "cashOnDelivery")
            {
                var order = new BasePackageModule2.Models.Order
                {
                    UserId = user.Id,
                    AddressId = add.Id,
                    PaymentStatus = "Cash on Delivery",
                    //TransactionId = transactionId,
                    Amount = subtotal,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = cart.Product.Id,
                        OrderId = order.Id,
                        Quantity = cart.Qty
                    };
                    _context.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
                _context.RemoveRange(carts);
                await _context.SaveChangesAsync();

                return View(nameof(ThankYou));
            }
            return BadRequest();

        }



        [HttpPost]
        public async Task<ActionResult> Complete(string rzp_paymentid, string rzp_orderid)
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

           var order = new BasePackageModule2.Models.Order
            {
                UserId = user.Id,
                TransactionId = orderId,
                Amount = Double.Parse(amt) / 100,
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

            foreach (var cart in await _context.Carts.Include(p => p.Product).Where(u => u.UserId == user.Id).ToListAsync())
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    ProductId = cart.Product.Id,
                    OrderId = order.Id,
                    Quantity = cart.Qty
                };
                _context.Add(orderProduct);
            }

            await _context.SaveChangesAsync();

            var carts = await _context.Carts.Where(u => u.UserId == user.Id).ToListAsync();
            _context.RemoveRange(carts);

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
        }
        public class PaymentModes
        {
            [Required]
            public string PaymentMode { get; set; }
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }
    }
}