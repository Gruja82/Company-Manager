using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class MaterialDto:BaseDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        [Required]
        public string Unit { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
