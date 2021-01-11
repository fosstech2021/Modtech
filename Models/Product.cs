using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace BasePackageModule2.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("Main Image")]
        public string Image { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public string Description { get; set; }


        [DisplayName("Short Description")]
        [MaxLength(200)]
        public string ShortDescription { get; set; }



        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Sub Category")]
        public int? SubCategoryId { get; set; }

        [DisplayName("Sub Category")]
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [DisplayName("Base Price")]


        public double? BasePrice { get; set; }


        [DisplayName("Discount Amount")]
        public double? DiscountAmount { get; set; }


        [DisplayName("Product Tags")]
        public string ProductTags { get; set; }

        [DisplayName("Free Shipping")]
        public bool FreeShipping { get; set; }

        public bool Status { get; set; }

        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }


        [DisplayName("Final Price")]
        public double? FinalPrice
        {
            get
            {
                if (DiscountAmount == null)
                {
                    if (BasePrice != null)
                    {
                        return Math.Round((double)BasePrice);
                    }

                    return null;
                }

                if (BasePrice != null)
                    return Math.Round((double)(BasePrice - DiscountAmount), 2);
                return BasePrice;
            }
        }
        [DisplayName("Discount Percentage")]
        public double? DiscountPercentage
        {
            get
            {
                if (DiscountAmount != null) return Math.Round((double)((DiscountAmount / BasePrice) * 100), 2);
                return 0;
            }
        }


        public double? RoundedBasePrice => Math.Round((double)BasePrice, 2);
    }
}
