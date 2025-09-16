using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_TipoMuestra")]
    public class TipoMuestra
    {
        [Key]
        [Column("IdTipoMuestra")]
        public int IdTipoMuestra { get; set; }

        [Required]
        [Column("Nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("Descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
