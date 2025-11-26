using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoZetino.WebMVC.Models // O tu namespace de DTOs
{
    public class MuestraDto
    {
        public int IdMuestra { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una Orden de Examen.")]
        [Display(Name = "Orden de Examen")]
        public int IdOrdenExamen { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un Tipo de Muestra.")]
        [Display(Name = "Tipo de Muestra")]
        public int IdTipoMuestra { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de recepción es obligatoria.")]
        [Display(Name = "Fecha de Recepción")]
        [DataType(DataType.Date)] // Ayuda a Razor a mostrar un selector de fecha
        public DateTime FechaRecepcion { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }


        // --- Propiedades Opcionales 
       public string NombrePacienteOrden { get; set; } 
       public string NombreTipoMuestra { get; set; }
    }
}