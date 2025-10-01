using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class TipoExamenRepository : ITipoExamenRepository
    {
        private readonly AppDBContext _context;

        public TipoExamenRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los tipos de examen
        public async Task<IEnumerable<TipoExamen>> GetTiposExamenAsync()
        {
            return await _context.TiposExamen.ToListAsync();
        }

        // Obtener tipo de examen por Id
        public async Task<TipoExamen> GetTipoExamenByIdAsync(int id)
        {
            return await _context.TiposExamen.FindAsync(id);
        }

        // Agregar un nuevo tipo de examen
        public async Task<TipoExamen> AddTipoExamenAsync(TipoExamen tipoExamen)
        {
            _context.TiposExamen.Add(tipoExamen);
            await _context.SaveChangesAsync();
            return tipoExamen;
        }

        // Actualizar tipo de examen existente
        public async Task<TipoExamen> UpdateTipoExamenAsync(TipoExamen tipoExamen)
        {
            var existingTipo = await _context.TiposExamen.FindAsync(tipoExamen.IdTipoExamen);
            if (existingTipo == null) return null;

            existingTipo.Nombre = tipoExamen.Nombre;
            existingTipo.Descripcion = tipoExamen.Descripcion;
            existingTipo.Precio = tipoExamen.Precio;
            existingTipo.Estado = tipoExamen.Estado;

            await _context.SaveChangesAsync();
            return existingTipo;
        }

        // Eliminar tipo de examen por Id
        public async Task<bool> DeleteTipoExamenAsync(int id)
        {
            var tipoExamen = await _context.TiposExamen.FindAsync(id);
            if (tipoExamen == null) return false;

            _context.TiposExamen.Remove(tipoExamen);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener tipo de examen por nombre
        public async Task<TipoExamen> GetTipoExamenByNombreAsync(string nombre)
        {
            return await _context.TiposExamen
                .FirstOrDefaultAsync(t => t.Nombre == nombre);
        }

        // Obtener tipos de examen por estado
        public async Task<IEnumerable<TipoExamen>> GetTiposExamenByEstadoAsync(bool estado)
        {
            return await _context.TiposExamen
                .Where(t => t.Estado == estado)
                .ToListAsync();
        }

        // Obtener tipos de examen por rango de precio
        public async Task<IEnumerable<TipoExamen>> GetTiposExamenByPrecioAsync(decimal precioMin, decimal precioMax)
        {
            return await _context.TiposExamen
                .Where(t => t.Precio >= precioMin && t.Precio <= precioMax)
                .ToListAsync();
        }
    }
}
