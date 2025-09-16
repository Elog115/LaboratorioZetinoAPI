using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
    [Table("t_NotificacionEmail")]
    public class NotificacionEmail
    {
        [Key]
        [Column("IdNotificacion")]
        public int IdNotificacion { get; set; }

        [Required]
        [Column("IdResultado")]
        public int IdResultado { get; set; }

        [Required]
        [Column("Asunto")]
        [StringLength(150)]
        public string Asunto { get; set; }

        [Required]
        [Column("Mensaje")]
        [StringLength(500)]
        public string Mensaje { get; set; }

        [Required]
        [Column("EstadoEnvio")]
        [StringLength(50)]
        public string EstadoEnvio { get; set; }

        [Required]
        [Column("Estado")]
        public int Estado { get; set; }
    }
}
