using Microsoft.EntityFrameworkCore;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Infrastructure.Data.Repositories
{
    public class NotificacionEmailRepository : INotificacionEmailRepository
    {
        private readonly AppDBContext _context;

        public NotificacionEmailRepository(AppDBContext context)
        {
            _context = context;
        }

        // Obtener todas las notificaciones
        public async Task<IEnumerable<NotificacionEmail>> GetNotificacionesAsync()
        {
            return await _context.NotificacionesEmail.ToListAsync();
        }

        // Obtener una notificación por su Id
        public async Task<NotificacionEmail> GetNotificacionByIdAsync(int id)
        {
            return await _context.NotificacionesEmail
                                 .FirstOrDefaultAsync(n => n.IdNotificacion == id);
        }

        // Agregar una nueva notificación
        public async Task<NotificacionEmail> AddNotificacionAsync(NotificacionEmail notificacion)
        {
            _context.NotificacionesEmail.Add(notificacion);
            await _context.SaveChangesAsync();
            return notificacion;
        }

        // Actualizar una notificación existente
        public async Task<NotificacionEmail> UpdateNotificacionAsync(NotificacionEmail notificacion)
        {
            _context.NotificacionesEmail.Update(notificacion);
            await _context.SaveChangesAsync();
            return notificacion;
        }

        // Eliminar una notificación por su Id
        public async Task<bool> DeleteNotificacionAsync(int id)
        {
            var notificacion = await _context.NotificacionesEmail.FindAsync(id);
            if (notificacion == null)
                return false;

            _context.NotificacionesEmail.Remove(notificacion);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener notificaciones por IdResultado
        public async Task<IEnumerable<NotificacionEmail>> GetNotificacionesByResultadoAsync(int idResultado)
        {
            return await _context.NotificacionesEmail
                                 .Where(n => n.IdResultado == idResultado)
                                 .ToListAsync();
        }

        // Obtener notificaciones por estado de envío
        public async Task<IEnumerable<NotificacionEmail>> GetNotificacionesByEstadoEnvioAsync(string estadoEnvio)
        {
            return await _context.NotificacionesEmail
                                 .Where(n => n.EstadoEnvio == estadoEnvio)
                                 .ToListAsync();
        }

        // Obtener notificaciones por estado
        public async Task<IEnumerable<NotificacionEmail>> GetNotificacionesByEstadoAsync(int estado)
        {
            return await _context.NotificacionesEmail
                                 .Where(n => n.Estado == estado)
                                 .ToListAsync();
        }
    }
}
