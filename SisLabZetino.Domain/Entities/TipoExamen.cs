using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_TipoExamen")]
    public class TipoExamen
    {
        [Key]
        [Column("IdTipoExamen")]
        public int IdTipoExamen { get; set; }

        [Required]
        [Column("Nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [Column("Descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("Precio")]
        public decimal Precio { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
