using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Data.Repositories
{
    public class MuestraRepository : IMuestraRepository
    {
        private readonly AppDBContext _context;

        public MuestraRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todas las muestras
        public async Task<IEnumerable<Muestra>> GetMuestrasAsync()
        {
            return await _context.Muestras.ToListAsync();
        }

        // Obtener una muestra por Id
        public async Task<Muestra> GetMuestraByIdAsync(int id)
        {
            return await _context.Muestras
                                 .FirstOrDefaultAsync(m => m.IdMuestra == id);
        }

        // Agregar una nueva muestra
        public async Task<Muestra> AddMuestraAsync(Muestra muestra)
        {
            _context.Muestras.Add(muestra);
            await _context.SaveChangesAsync();
            return muestra;
        }

        // Actualizar una muestra existente
        public async Task<Muestra> UpdateMuestraAsync(Muestra muestra)
        {
            _context.Muestras.Update(muestra);
            await _context.SaveChangesAsync();
            return muestra;
        }


        // Obtener muestras por IdOrdenExamen
        public async Task<IEnumerable<Muestra>> GetMuestrasByOrdenAsync(int idOrdenExamen)
        {
            return await _context.Muestras
                                 .Where(m => m.IdOrdenExamen == idOrdenExamen)
                                 .ToListAsync();
        }

        // Obtener muestras por IdTipoMuestra
        public async Task<IEnumerable<Muestra>> GetMuestrasByTipoAsync(int idTipoMuestra)
        {
            return await _context.Muestras
                                 .Where(m => m.IdTipoMuestra == idTipoMuestra)
                                 .ToListAsync();
        }

        // Obtener muestras por Estado
        public async Task<IEnumerable<Muestra>> GetMuestrasByEstadoAsync(bool estado)
        {
            return await _context.Muestras
                                 .Where(m => m.Estado == estado)
                                 .ToListAsync();
        }

        // Obtener muestras por Fecha de Recepción
        public async Task<IEnumerable<Muestra>> GetMuestrasByFechaRecepcionAsync(DateTime fechaRecepcion)
        {
            // Se compara solo la parte de la fecha, ignorando la hora
            return await _context.Muestras
                                 .Where(m => m.FechaRecepcion.Date == fechaRecepcion.Date)
                                 .ToListAsync();
        }
    }
}
