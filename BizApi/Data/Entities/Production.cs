using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class Production
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int QtyProduct { get; set; }
        public virtual Product Product { get; set; } = default!;
    }
}
