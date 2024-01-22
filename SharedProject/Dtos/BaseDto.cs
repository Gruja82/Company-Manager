using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class BaseDto
    {
        [Required]
        public int Id { get; set; }
        public IFormFile? Image { get; set; }
    }
}
