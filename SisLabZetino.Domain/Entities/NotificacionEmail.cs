using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_notificacionemail")]
    public class NotificacionEmail
    {
        [Key]
        [Column("idnotificacion")]
        public int IdNotificacion { get; set; }

        [Required]
        [Column("idresultado")]
        public int IdResultado { get; set; }

        [Required]
        [Column("asunto")]
        [StringLength(150)]
        public string Asunto { get; set; }

        [Required]
        [Column("mensaje")]
        [StringLength(500)]
        public string Mensaje { get; set; }

        [Required]
        [Column("estadoenvio")]
        [StringLength(50)]
        public string EstadoEnvio { get; set; }

        [Required]
        [Column("estado")]
        public bool Estado { get; set; }
    }
}
