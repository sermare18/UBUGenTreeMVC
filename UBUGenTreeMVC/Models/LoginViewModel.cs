using System;
using System.ComponentModel.DataAnnotations;

namespace UBUGenTreeMVC.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string _email { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        public string _contrasenaHash { get; set; }
    }
}

