using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class OrderDto:BaseDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderDetailDto> OrderDetailDtos { get; set; } = new();
    }
}
