using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_rol")]
    public class Rol
    {
        [Key]
        [Column("idrol")]
        public int IdRol { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Column("descripcion")] 
        [StringLength(500)]
        public string? Descripcion { get; set; } // La hacemos 'nullable' (string?)

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}