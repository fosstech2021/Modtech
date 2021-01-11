using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasePackageModule2.Models
{
    public class Order
    {
        public int Id { get; set; }
      
        public virtual Address Address { get; set; }
        public int? AddressId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Display(Name = "Payment Id")]
        public string PaymentId { get; set; }
        [Display(Name = "Transaction Id")]
        public string TransactionId { get; set; }
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }
        public double Amount { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Date { get; set; }

        public IEnumerable<OrderProduct> OrderProducts { get; set; }

        [NotMapped]
        [Display(Name = "Products")]
        public List<int> Products { get; set; }
    }
}