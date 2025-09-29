using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Data.Repositories
{
    public class ExamenRepository : IExamenRepository
    {
        private readonly AppDBContext _context;

        public ExamenRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todos los exámenes
        public async Task<IEnumerable<Examen>> GetExamenesAsync()
        {
            return await _context.Examenes.ToListAsync();
        }

        // Obtener examen por Id
        public async Task<Examen> GetExamenByIdAsync(int id)
        {
            return await _context.Examenes
                                 .FirstOrDefaultAsync(e => e.IdExamen == id);
        }

        // Agregar examen
        public async Task<Examen> AddExamenAsync(Examen examen)
        {
            _context.Examenes.Add(examen);
            await _context.SaveChangesAsync();
            return examen;
        }

        // Actualizar examen
        public async Task<Examen> UpdateExamenAsync(Examen examen)
        {
            _context.Examenes.Update(examen);
            await _context.SaveChangesAsync();
            return examen;
        }

        // Eliminar examen
        public async Task<bool> DeleteExamenAsync(int id)
        {
            var examen = await _context.Examenes.FindAsync(id);
            if (examen == null)
                return false;

            _context.Examenes.Remove(examen);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener exámenes por IdOrdenExamen
        public async Task<IEnumerable<Examen>> GetExamenesByOrdenAsync(int idOrdenExamen)
        {
            return await _context.Examenes
                                 .Where(e => e.IdOrdenExamen == idOrdenExamen)
                                 .ToListAsync();
        }

        // Obtener exámenes por IdTipoExamen
        public async Task<IEnumerable<Examen>> GetExamenesByTipoAsync(int idTipoExamen)
        {
            return await _context.Examenes
                                 .Where(e => e.IdTipoExamen == idTipoExamen)
                                 .ToListAsync();
        }

        // Obtener exámenes por Estado
        public async Task<IEnumerable<Examen>> GetExamenesByEstadoAsync(bool estado)
        {
            return await _context.Examenes
                                 .Where(e => e.Estado == estado)
                                 .ToListAsync();
        }
    }
}
