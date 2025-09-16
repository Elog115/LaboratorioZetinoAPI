using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_UsuarioSistema")]
    public class UsuarioSistema
    {
        [Key]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("IdRol")]
        public int IdRol { get; set; }

        [Required]
        [Column("Nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("Apellido")]
        [StringLength(150)]
        public string Apellido { get; set; }

        [Required]
        [Column("Telefono")]
        [StringLength(8)]
        public string Telefono { get; set; }

        [Required]
        [Column("Correo")]
        [StringLength(250)]
        public string Correo { get; set; }

        [Required]
        [Column("FechaNacimiento", TypeName = "date")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Column("Clave")]
        [StringLength(250)]
        public string Clave { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
