using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class ProductDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public double QtyMaterial { get; set; }
        public virtual Product Product { get; set; } = default!;
        public virtual Material Material { get; set; } = default!;
    }
}
