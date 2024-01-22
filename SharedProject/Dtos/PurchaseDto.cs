using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class PurchaseDto:BaseDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public List<PurchaseDetailDto> PurchaseDetailDtos { get; set; } = new();
    }
}
