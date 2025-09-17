using Domains.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Category { get; set; }

        [Required, MaxLength(20)]
        public string ProductCode { get; set; } 

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string ImagePath { get; set; } 

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int MinimumQuantity { get; set; }

        public double DiscountRate { get; set; }
    }
}
