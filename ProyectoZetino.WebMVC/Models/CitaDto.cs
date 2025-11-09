using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class CitaDto
    {
        public int IdCita { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio")]
        [Display(Name = "Paciente")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y hora")]
        public DateTime? FechaHora { get; set; }

        [Required(ErrorMessage = "La descripción/motivo es obligatoria")]
        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        [Display(Name = "Motivo / Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Activa")]
        public bool Estado { get; set; }

        // Opcional: para mostrar en vistas sin otro fetch
        [Display(Name = "Paciente")]
        public string? NombreUsuario { get; set; }
    }
}
