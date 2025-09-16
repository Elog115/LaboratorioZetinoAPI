using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_Examen")]
    public class Examen
    {
        [Key]
        [Column("IdExamen")]
        public int IdExamen { get; set; }

        [Required]
        [Column("IdOrdenExamen")]
        public int IdOrdenExamen { get; set; }

        [Required]
        [Column("IdTipoExamen")]
        public int IdTipoExamen { get; set; }

        [Required]
        [Column("Descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required]
        [Column("TiempoEstimado")]
        public int TiempoEstimado { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
