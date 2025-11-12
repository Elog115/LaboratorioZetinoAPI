using System;
using System.ComponentModel.DataAnnotations; 

namespace ProyectoZetino.WebMVC.Models
{
    public class TipoMuestraDto
    {

        public int IdTipoMuestra { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre del Tipo")] 
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
        [Display(Name = "Descripción")] 
        public string Descripcion { get; set; }

        [Display(Name = "Activo")] 
        public bool Estado { get; set; }
    }
}