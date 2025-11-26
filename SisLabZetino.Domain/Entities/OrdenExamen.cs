using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_ordenexamen")]
    public class OrdenExamen
    {
        [Key]
        [Column("idordenexamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("idusuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("idcita")]
        public int IdCita { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("fechasolicitud", TypeName = "date")]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
