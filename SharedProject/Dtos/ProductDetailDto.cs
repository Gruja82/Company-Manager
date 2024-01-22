using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public double MaterialQty { get; set; }
        public double MaterialPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
    }
}
