using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_resultado")]
    public class Resultado
    {
        [Key]
        [Column("idresultado")]
        public int IdResultado { get; set; }

        [Required]
        [Column("idexamen")]
        public int IdExamen { get; set; }

        [Required]
        [Column("fechaentrega", TypeName = "date")]
        public DateTime FechaEntrega { get; set; }

        [Required]
        [Column("observaciones")]
        [StringLength(250)]
        public string Observaciones { get; set; }

        [Required]
        [Column("archivoresultado")]
        [StringLength(250)]
        public string ArchivoResultado { get; set; }

        [Required]
        [Column("estado")]
        public int Estado { get; set; }
    }
}
