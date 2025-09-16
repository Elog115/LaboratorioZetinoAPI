using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_Cita")]
    public class Cita
    {
        [Key]
        [Column("IdCita")]
        public int IdCita { get; set; }

        [Required]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("FechaHora", TypeName = "datetime")]
        public DateTime FechaHora { get; set; }

        [Required]
        [Column("Descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
