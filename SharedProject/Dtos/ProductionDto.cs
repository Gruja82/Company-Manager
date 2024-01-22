using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class ProductionDto:BaseDto
    {
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        [Required]
        public int Quantity { get; set; }
    }
}
