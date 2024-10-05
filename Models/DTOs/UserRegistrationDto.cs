using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Models.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public required string Email {get; set;}

        [Required]
        public required string Password {get; set;}
    }
}