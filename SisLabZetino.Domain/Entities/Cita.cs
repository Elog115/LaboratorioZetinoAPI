using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_cita")]
    public class Cita
    {
        [Key]
        [Column("idcita")]
        public int IdCita { get; set; }

        [Required]
        [Column("idusuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("fechahora", TypeName = "datetime")]
        public DateTime FechaHora { get; set; }

        [Required]
        [Column("descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
