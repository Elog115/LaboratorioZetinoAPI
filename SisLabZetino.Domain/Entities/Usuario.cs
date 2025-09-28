using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_usuario")]
    public class Usuario
    {
        [Key]
        [Column("idusuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("idrol")]
        public int IdRol { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("apellido")]
        [StringLength(150)]
        public string Apellido { get; set; }

        [Required]
        [Column("telefono")]
        [StringLength(8)]
        public string Telefono { get; set; }

        [Required]
        [Column("correo")]
        [StringLength(250)]
        public string Correo { get; set; }

        [Required]
        [Column("fechanacimiento", TypeName = "date")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Column("clave")]
        [StringLength(250)]
        public string Clave { get; set; }

        [Required]
        [Column("estado")]
        public int Estado { get; set; }
    }
}
