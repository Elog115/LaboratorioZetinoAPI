using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_Muestra")]
    public class Muestra
    {
        [Key]
        [Column("IdMuestra")]
        public int IdMuestra { get; set; }

        [Required]
        [Column("IdOrdenExamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("IdTipoMuestra")]
        public int IdTipoMuestra { get; set; }

        [Required]
        [Column("FechaRecepcion", TypeName = "date")]
        public DateTime FechaRecepcion { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
