﻿using System.ComponentModel.DataAnnotations;

namespace Datatable.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}