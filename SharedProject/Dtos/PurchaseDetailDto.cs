using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class PurchaseDetailDto
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public double Quantity { get; set; }
        public double MaterialPrice { get; set; }
        public string MaterialName { get; set; } = string.Empty;
    }
}
