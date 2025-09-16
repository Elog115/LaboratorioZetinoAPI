using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_OrdenExamen")]
    public class OrdenExamen
    {
        [Key]
        [Column("IdOrdenExamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("IdCita")]
        public int IdCita { get; set; }

        [Required]
        [Column("FechaSolicitud", TypeName = "date")]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
