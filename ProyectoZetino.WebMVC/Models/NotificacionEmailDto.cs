using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class NotificacionEmailDto
    {
        public int IdNotificacion { get; set; }

        [Required]
        [Display(Name = "ID Resultado")]
        public int IdResultado { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(150)]
        public string Asunto { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)] // Para que use <textarea> si lo editamos
        public string Mensaje { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Estado de Envío")]
        public string EstadoEnvio { get; set; } // "Pendiente", "Enviado", "Error"

        [Display(Name = "Activo")]
        public bool Estado { get; set; }
    }
}