using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class Material
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
        public double Qty { get; set; }
        [Required]
        public string Unit { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = default!;
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = default!;
    }
}
