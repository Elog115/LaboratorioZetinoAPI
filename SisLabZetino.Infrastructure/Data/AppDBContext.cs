
using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Data
    {
        public class AppDBContext : DbContext
        {
            public AppDBContext(DbContextOptions<AppDBContext> options)
                : base(options)
            {
            }

            // DbSets = representan las tablas en la BD
            public DbSet<UsuarioSistema> UsuariosSistema { get; set; }
            public DbSet<Rol> Roles { get; set; }
            public DbSet<Cita> Citas { get; set; }
            public DbSet<Examen> Examenes { get; set; }
            public DbSet<Muestra> Muestras { get; set; }
            public DbSet<NotificacionEmail> NotificacionesEmail { get; set; }
            public DbSet<OrdenExamen> OrdenesExamen { get; set; }
            public DbSet<Resultado> Resultados { get; set; }
            public DbSet<TipoExamen> TiposExamen { get; set; }
            public DbSet<TipoMuestra> TiposMuestra { get; set; }





            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Mapeos de entidades a tablas
                modelBuilder.Entity<UsuarioSistema>().ToTable("t_usuario_sistema");
                modelBuilder.Entity<Rol>().ToTable("t_rol");
                modelBuilder.Entity<Cita>().ToTable("t_Cita");
                modelBuilder.Entity<Examen>().ToTable("t_Examen");
                modelBuilder.Entity<Muestra>().ToTable("t_Muestra");
                modelBuilder.Entity<NotificacionEmail>().ToTable("t_NotificacionEmail");
                modelBuilder.Entity<OrdenExamen>().ToTable("t_OrdenExamen");
                modelBuilder.Entity<Resultado>().ToTable("t_Resultado");
            modelBuilder.Entity<TipoExamen>().ToTable("t_TipoExamen");
            modelBuilder.Entity<TipoMuestra>().ToTable("t_TipoMuestra");


            base.OnModelCreating(modelBuilder);
            }
        }
    }



