using System;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class NewsSubscriber
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        public bool? Status { get; set; }
        public DateTime Created { get; set; }
    }
}