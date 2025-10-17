using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDBContext _ctx;
        public UsuarioRepository(AppDBContext ctx) => _ctx = ctx;

        // Obtener todos los usuarios
        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
            => await _ctx.Usuarios.AsNoTracking().ToListAsync();

        // Obtener usuario por Id
        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
            => await _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);

        public async Task<Usuario?> GetByEmailAsync(string email)
            => await _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);


        // Obtener usuarios por IdRol
        public async Task<IEnumerable<Usuario>> GetUsuariosByRolAsync(int idRol)
            => await _ctx.Usuarios.AsNoTracking().Where(u => u.IdRol == idRol).ToListAsync();

        // Obtener usuarios por estado
        public async Task<IEnumerable<Usuario>> GetUsuariosByEstadoAsync(bool estado)
            => await _ctx.Usuarios.AsNoTracking().Where(u => u.Estado == estado).ToListAsync();

        // Agregar un nuevo usuario
        public async Task<Usuario> AddUsuarioAsync(Usuario usuario)
        {
            _ctx.Usuarios.Add(usuario);
            await _ctx.SaveChangesAsync();
            return usuario;
        }

        // Actualizar usuario existente
        public async Task<Usuario?> UpdateUsuarioAsync(Usuario usuario)
        {
            var existingUsuario = await _ctx.Usuarios.FindAsync(usuario.IdUsuario);
            if (existingUsuario == null) return null;

            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Apellido = usuario.Apellido;
            existingUsuario.Telefono = usuario.Telefono;
            existingUsuario.Email = usuario.Email;
            existingUsuario.FechaNacimiento = usuario.FechaNacimiento;
            existingUsuario.PasswordHash = usuario.PasswordHash;
            existingUsuario.IdRol = usuario.IdRol;
            existingUsuario.Estado = usuario.Estado;

            await _ctx.SaveChangesAsync();
            return existingUsuario;
        }

        // Eliminar usuario por Id
        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _ctx.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _ctx.Usuarios.Remove(usuario);
            await _ctx.SaveChangesAsync();
            return true;
        }

        // Validar usuario (login)
        public async Task<Usuario?> ValidateUsuarioAsync(string email, string password)
            => await _ctx.Usuarios.AsNoTracking()
                                  .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
    }
}
