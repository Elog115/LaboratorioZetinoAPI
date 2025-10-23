
using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class RegisterModel
    {
        [Required]
        public string Nombre { get; set; } = string.Empty; 

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Telefono { get; set; } = string.Empty; 

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; 

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; 
    }
}