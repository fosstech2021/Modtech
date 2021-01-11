using System;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class Update
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }
        [Display(Name = "Meta description")]
        public string MetaDescription { get; set; }
        [Display(Name = "Meta keywords")]
        public string MetaKeywords { get; set; }

        public string Image { get; set; }

        [Required]
        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedDate { get; set; }

        public Update()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
