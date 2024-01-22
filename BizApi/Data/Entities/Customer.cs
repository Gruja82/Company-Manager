using System.ComponentModel.DataAnnotations;

namespace BizApi.Data.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Contact { get; set; } = default!;
        [Required]
        public string Address { get; set; } = default!;
        [Required]
        public string City { get; set; } = default!;
        [Required]
        public string Postal { get; set; } = default!;
        [Required]
        public string Phone { get; set; } = default!;
        [Required]
        public string Email { get; set; } = default!;
        public string? Web { get; set; } = default!;
        public string ImageUrl { get; set; } = string.Empty;
        public virtual ICollection<Order> Orders { get; set; } = default!;
    }
}
