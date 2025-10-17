using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Entities
{
        [Table("t_usuario")]
        public class Usuario
        {
            [Key]
            [Column("idusuario")]
            public int IdUsuario { get; set; }

            [Required]
            [Column("idrol")]
            public int IdRol { get; set; }

            [Required]
            [Column("nombre")]
            [StringLength(150)]
            public string Nombre { get; set; } = string.Empty;

            [Required]
            [Column("apellido")]
            [StringLength(150)]
            public string Apellido { get; set; } = string.Empty;

            [Required]
            [Column("telefono")]
            [StringLength(8)]
            public string Telefono { get; set; } = string.Empty;

            [Required]
            [Column("email")]
            [StringLength(250)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Column("fechanacimiento", TypeName = "date")]
            public DateTime FechaNacimiento { get; set; }

            // Este campo almacena la contraseña cifrada (en BD)
            [Required]
            [Column("password")]
            [StringLength(250)]
            public string PasswordHash { get; set; } = string.Empty;

            // Campo temporal, NO se guarda en BD (solo para registro/login)
            [NotMapped]
            public string? Password { get; set; }

            [Required]
            [Column("estado")]
            public bool Estado { get; set; }

            // Relación con Rol (opcional, si tienes tabla de roles)
            [ForeignKey("idrol")]
            public Rol? Rol { get; set; }
        }
    }


