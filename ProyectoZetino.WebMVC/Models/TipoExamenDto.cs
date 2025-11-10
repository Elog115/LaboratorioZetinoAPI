using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class TipoExamenDto
    {
        public int IdTipoExamen { get; set; }

        [Required(ErrorMessage = "El nombre del examen es obligatorio")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        [Display(Name = "Nombre del Examen")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio ($)")]
        public decimal Precio { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }
    }
}

