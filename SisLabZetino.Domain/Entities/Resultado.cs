using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_Resultado")]
    public class Resultado
    {
        [Key]
        [Column("IdResultado")]
        public int IdResultado { get; set; }

        [Required]
        [Column("IdExamen")]
        public int IdExamen { get; set; }

        [Required]
        [Column("FechaEntrega", TypeName = "date")]
        public DateTime FechaEntrega { get; set; }

        [Required]
        [Column("Observaciones")]
        [StringLength(250)]
        public string Observaciones { get; set; }

        [Required]
        [Column("ArchivoResultado")]
        [StringLength(250)]
        public string ArchivoResultado { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
