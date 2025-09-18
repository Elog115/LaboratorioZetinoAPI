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
    public class RolRepository : IRolRepository
    {
        private readonly AppDBContext _context;

        public RolRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los roles
        public async Task<IEnumerable<Rol>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Obtener un rol por Id
        public async Task<Rol> GetRolByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        // Agregar un nuevo rol
        public async Task<Rol> AddRolAsync(Rol rol)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        // Actualizar un rol existente
        public async Task<Rol> UpdateRolAsync(Rol rol)
        {
            var existingRol = await _context.Roles.FindAsync(rol.IdRol);
            if (existingRol == null)
            {
                return null;
            }

            existingRol.Nombre = rol.Nombre;
            existingRol.Estado = rol.Estado;

            await _context.SaveChangesAsync();
            return existingRol;
        }

        // Eliminar un rol por Id
        public async Task<bool> DeleteRolAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return false;
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener roles por estado
        public async Task<IEnumerable<Rol>> GetRolesByEstadoAsync(int estado)
        {
            return await _context.Roles
                                 .Where(r => r.Estado == estado)
                                 .ToListAsync();
        }

        // Obtener rol por nombre
        public async Task<Rol> GetRolByNombreAsync(string nombre)
        {
            return await _context.Roles
                                 .FirstOrDefaultAsync(r => r.Nombre == nombre);
        }
    }
}

