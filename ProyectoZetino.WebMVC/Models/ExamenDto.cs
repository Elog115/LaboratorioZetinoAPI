using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class ExamenDto
    {
        public int IdExamen { get; set; }

        [Required(ErrorMessage = "La orden de examen es obligatoria")]
        [Display(Name = "Orden de Examen")]
        public int IdOrdenExamen { get; set; }

        [Required(ErrorMessage = "El tipo de examen es obligatorio")]
        [Display(Name = "Tipo de Examen")]
        public int IdTipoExamen { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tiempo estimado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ser un valor mayor a 0")]
        [Display(Name = "Tiempo Estimado (minutos)")]
        public int TiempoEstimado { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }

        // Campos opcionales para mostrar en vistas sin hacer consultas adicionales
        [Display(Name = "Nombre de la Orden")]
        public string? NombreOrden { get; set; }

        [Display(Name = "Tipo de Examen")]
        public string? NombreTipoExamen { get; set; }
    }
}

