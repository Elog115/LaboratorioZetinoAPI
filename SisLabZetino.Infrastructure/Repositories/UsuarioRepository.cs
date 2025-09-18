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
    public class UsuarioSistemaRepository : IUsuarioSistemaRepository
    {
        private readonly AppDBContext _context;

        public UsuarioSistemaRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<UsuarioSistema>> GetUsuariosAsync()
        {
            return await _context.UsuariosSistema.ToListAsync();
        }

        // Obtener usuario por Id
        public async Task<UsuarioSistema> GetUsuarioByIdAsync(int id)
        {
            return await _context.UsuariosSistema.FindAsync(id);
        }

        // Agregar un nuevo usuario
        public async Task<UsuarioSistema> AddUsuarioAsync(UsuarioSistema usuario)
        {
            _context.UsuariosSistema.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Actualizar usuario existente
        public async Task<UsuarioSistema> UpdateUsuarioAsync(UsuarioSistema usuario)
        {
            var existingUsuario = await _context.UsuariosSistema.FindAsync(usuario.IdUsuario);
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
            var usuario = await _context.UsuariosSistema.FindAsync(id);
            if (usuario == null)
            {
                return false;
            }

            _context.UsuariosSistema.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener usuarios por IdRol
        public async Task<IEnumerable<UsuarioSistema>> GetUsuariosByRolAsync(int idRol)
        {
            return await _context.UsuariosSistema
                                 .Where(u => u.IdRol == idRol)
                                 .ToListAsync();
        }

        // Obtener usuarios por estado (activo/inactivo)
        public async Task<IEnumerable<UsuarioSistema>> GetUsuariosByEstadoAsync(int estado)
        {
            return await _context.UsuariosSistema
                                 .Where(u => u.Estado == estado)
                                 .ToListAsync();
        }

        // Obtener usuario por correo
        public async Task<UsuarioSistema> GetUsuarioByCorreoAsync(string correo)
        {
            return await _context.UsuariosSistema
                                 .FirstOrDefaultAsync(u => u.Correo == correo);
        }

        // Validar usuario (autenticación)
        public async Task<UsuarioSistema> ValidateUsuarioAsync(string correo, string clave)
        {
            return await _context.UsuariosSistema
                                 .FirstOrDefaultAsync(u => u.Correo == correo && u.Clave == clave);
        }
    }
}
