using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Referencias
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDBContext _context;

        public UsuarioRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Obtener usuario por Id
        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // Agregar un nuevo usuario
        public async Task<Usuario> AddUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Actualizar usuario existente
        public async Task<Usuario> UpdateUsuarioAsync(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            if (existingUsuario == null)
            {
                return null;
            }

            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Apellido = usuario.Apellido;
            existingUsuario.Correo = usuario.Correo;
            existingUsuario.Clave = usuario.Clave;
            existingUsuario.IdRol = usuario.IdRol;
            existingUsuario.Estado = usuario.Estado;

            await _context.SaveChangesAsync();
            return existingUsuario;
        }

        // Eliminar usuario por Id
        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener usuarios por IdRol
        public async Task<IEnumerable<Usuario>> GetUsuariosByRolAsync(int idRol)
        {
            return await _context.Usuarios
                                 .Where(u => u.IdRol == idRol)
                                 .ToListAsync();
        }

        // Obtener usuarios por estado (activo/inactivo)
        public async Task<IEnumerable<Usuario>> GetUsuariosByEstadoAsync(bool estado)
        {
            return await _context.Usuarios
                                 .Where(u => u.Estado == estado)
                                 .ToListAsync();
        }

       
    }
}
