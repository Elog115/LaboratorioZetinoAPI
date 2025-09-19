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
    public class OrdenExamenRepository : IOrdenExamenRepository
    {
        private readonly AppDBContext _context;

        public OrdenExamenRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todas las órdenes de examen
        public async Task<IEnumerable<OrdenExamen>> GetOrdenesExamenAsync()
        {
            return await _context.OrdenesExamen.ToListAsync();
        }

        // Obtener una orden de examen por su Id
        public async Task<OrdenExamen> GetOrdenExamenByIdAsync(int id)
        {
            return await _context.OrdenesExamen
                                 .FirstOrDefaultAsync(o => o.IdOrdenExamen == id);
        }

        // Agregar una nueva orden de examen
        public async Task<OrdenExamen> AddOrdenExamenAsync(OrdenExamen ordenExamen)
        {
            _context.OrdenesExamen.Add(ordenExamen);
            await _context.SaveChangesAsync();
            return ordenExamen;
        }

        // Actualizar una orden de examen existente
        public async Task<OrdenExamen> UpdateOrdenExamenAsync(OrdenExamen ordenExamen)
        {
            _context.OrdenesExamen.Update(ordenExamen);
            await _context.SaveChangesAsync();
            return ordenExamen;
        }

        // Eliminar una orden de examen por su Id
        public async Task<bool> DeleteOrdenExamenAsync(int id)
        {
            var orden = await _context.OrdenesExamen.FindAsync(id);
            if (orden == null)
                return false;

            _context.OrdenesExamen.Remove(orden);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener órdenes de examen por IdUsuario
        public async Task<IEnumerable<OrdenExamen>> GetOrdenesByUsuarioAsync(int idUsuario)
        {
            return await _context.OrdenesExamen
                                 .Where(o => o.IdUsuario == idUsuario)
                                 .ToListAsync();
        }

        // Obtener órdenes de examen por IdCita
        public async Task<IEnumerable<OrdenExamen>> GetOrdenesByCitaAsync(int idCita)
        {
            return await _context.OrdenesExamen
                                 .Where(o => o.IdCita == idCita)
                                 .ToListAsync();
        }

        // Obtener órdenes de examen por fecha de solicitud
        public async Task<IEnumerable<OrdenExamen>> GetOrdenesByFechaSolicitudAsync(DateTime fechaSolicitud)
        {
            // Se compara solo la parte de la fecha
            return await _context.OrdenesExamen
                                 .Where(o => o.FechaSolicitud.Date == fechaSolicitud.Date)
                                 .ToListAsync();
        }

        // Obtener órdenes de examen por estado
        public async Task<IEnumerable<OrdenExamen>> GetOrdenesByEstadoAsync(int estado)
        {
            return await _context.OrdenesExamen
                                 .Where(o => o.Estado == estado)
                                 .ToListAsync();
        }
    }
}
