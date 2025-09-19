using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Repositories
{
    public class TipoMuestraRepository : ITipoMuestraRepository
    {
        private readonly AppDBContext _context;

        public TipoMuestraRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los tipos de muestra
        public async Task<IEnumerable<TipoMuestra>> GetTiposMuestraAsync()
        {
            return await _context.TiposMuestra.ToListAsync();
        }

        // Obtener tipo de muestra por Id
        public async Task<TipoMuestra> GetTipoMuestraByIdAsync(int id)
        {
            return await _context.TiposMuestra.FindAsync(id);
        }

        // Agregar un nuevo tipo de muestra
        public async Task<TipoMuestra> AddTipoMuestraAsync(TipoMuestra tipoMuestra)
        {
            _context.TiposMuestra.Add(tipoMuestra);
            await _context.SaveChangesAsync();
            return tipoMuestra;
        }

        // Actualizar tipo de muestra existente
        public async Task<TipoMuestra> UpdateTipoMuestraAsync(TipoMuestra tipoMuestra)
        {
            var existingTipo = await _context.TiposMuestra.FindAsync(tipoMuestra.IdTipoMuestra);
            if (existingTipo == null) return null;

            existingTipo.Nombre = tipoMuestra.Nombre;
            existingTipo.Descripcion = tipoMuestra.Descripcion;
            existingTipo.Estado = tipoMuestra.Estado;

            await _context.SaveChangesAsync();
            return existingTipo;
        }

        // Eliminar tipo de muestra por Id
        public async Task<bool> DeleteTipoMuestraAsync(int id)
        {
            var tipoMuestra = await _context.TiposMuestra.FindAsync(id);
            if (tipoMuestra == null) return false;

            _context.TiposMuestra.Remove(tipoMuestra);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener tipo de muestra por nombre
        public async Task<TipoMuestra> GetTipoMuestraByNombreAsync(string nombre)
        {
            return await _context.TiposMuestra
                .FirstOrDefaultAsync(t => t.Nombre == nombre);
        }

        // Obtener tipos de muestra por estado
        public async Task<IEnumerable<TipoMuestra>> GetTiposMuestraByEstadoAsync(int estado)
        {
            return await _context.TiposMuestra
                .Where(t => t.Estado == estado)
                .ToListAsync();
        }
    }
}
