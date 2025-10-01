using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de notificaciones por email
    public interface INotificacionEmailRepository
    {
        //Obtener todas las notificaciones
        Task<IEnumerable<NotificacionEmail>> GetNotificacionesAsync();

        //Obtener una notificación por su Id
        Task<NotificacionEmail> GetNotificacionByIdAsync(int id);

        //Agregar una nueva notificación
        Task<NotificacionEmail> AddNotificacionAsync(NotificacionEmail notificacion);

        //Actualizar una notificación existente
        Task<NotificacionEmail> UpdateNotificacionAsync(NotificacionEmail notificacion);

        //Eliminar una notificación por su Id
        Task<bool> DeleteNotificacionAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener notificaciones por IdResultado
        Task<IEnumerable<NotificacionEmail>> GetNotificacionesByResultadoAsync(int idResultado);

        //Obtener notificaciones por estado de envío (ejemplo: Enviado, Fallido, Pendiente)
        Task<IEnumerable<NotificacionEmail>> GetNotificacionesByEstadoEnvioAsync(string estadoEnvio);

        //Obtener notificaciones por estado (ejemplo: activas/inactivas)
        Task<IEnumerable<NotificacionEmail>> GetNotificacionesByEstadoAsync(bool estado);
    }
}

