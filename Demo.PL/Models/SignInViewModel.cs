﻿using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
