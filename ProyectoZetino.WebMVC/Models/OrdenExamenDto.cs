using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class OrdenExamenDto
    {
        public int IdOrdenExamen { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Paciente")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La cita es obligatoria")]
        [Display(Name = "Cita")]
        public int IdCita { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de solicitud es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Solicitud")]
        public DateTime FechaSolicitud { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }

        // =======================
        // Campos opcionales para vistas
        // =======================

        [Display(Name = "Nombre del Paciente")]
        public string? NombreUsuario { get; set; }

        [Display(Name = "Detalles de la Cita")]
        public string? DetalleCita { get; set; }
    }
}
