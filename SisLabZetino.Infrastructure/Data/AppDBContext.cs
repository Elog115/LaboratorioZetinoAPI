
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
            public DbSet<Rol> Roles { get; set; }   // 👈 Ya agregado para que funcione RolRepository

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Mapeos de entidades a tablas
                modelBuilder.Entity<UsuarioSistema>().ToTable("t_usuario_sistema");
                modelBuilder.Entity<Rol>().ToTable("t_rol");   // 👈 Ya agregado para la entidad Rol

                base.OnModelCreating(modelBuilder);
            }
        }
    }



