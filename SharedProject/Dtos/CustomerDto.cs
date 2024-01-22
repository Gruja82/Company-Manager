using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class CustomerDto:BaseDto
    {
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
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string? Web { get; set; } = default!;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
