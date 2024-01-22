using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int Qty { get; set; }
        [Required]
        public string Unit { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = default!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = default!;
        public virtual ICollection<Production> Productions { get; set; } = default!;
    }
}
