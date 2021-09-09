using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.ViewModel.AppUsers
{
    public class UserRegisterRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string PasswordConfirm { get; set; }

        [Required]
        public IFormFile Avatar { get; set; }
    }
}
