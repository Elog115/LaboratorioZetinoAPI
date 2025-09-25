using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_tipomuestra")]
    public class TipoMuestra
    {
        [Key]
        [Column("idtipomuestra")]
        public int IdTipoMuestra { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("estado")]
        public int Estado { get; set; }
    }
}
