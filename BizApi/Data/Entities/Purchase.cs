using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; } = default!;
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = default!;
    }
}
