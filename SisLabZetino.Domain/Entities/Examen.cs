using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_examen")]
    public class Examen
    {
        [Key]
        [Column("idexamen")]
        public int IdExamen { get; set; }

        [Required]
        [Column("idordenexamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("idtipoexamen")]
        public int IdTipoExamen { get; set; }

        [Required]
        [Column("descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("tiempoestimado")]
        public int TiempoEstimado { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
