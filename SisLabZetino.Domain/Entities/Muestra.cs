using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_muestra")]
    public class Muestra
    {
        [Key]
        [Column("idmuestra")]
        public int IdMuestra { get; set; }

        [Required]
        [Column("idordenexamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("idtipomuestra")]
        public int IdTipoMuestra { get; set; }

        [Required]
        [Column("fecharecepcion", TypeName = "date")]
        public DateTime FechaRecepcion { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
