using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models
{
    public class ResultadoDto
    {
        public int IdResultado { get; set; }

        [Required(ErrorMessage = "El examen es obligatorio.")]
        [Display(Name = "Examen")]
        public int IdExamen { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria.")]
        [Display(Name = "Fecha de Entrega")]
        [DataType(DataType.Date)]
        public DateTime FechaEntrega { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(250, ErrorMessage = "Las observaciones no deben exceder los 250 caracteres.")]
        public string? Observaciones { get; set; } // Permitir nulo si es opcional

        [Required(ErrorMessage = "El archivo de resultado es obligatorio.")]
        [Display(Name = "Archivo (Ruta)")]
        [StringLength(250)]
        public string ArchivoResultado { get; set; }

        public bool Estado { get; set; }
    }
}