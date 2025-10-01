using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase) para Notificaciones
    public class NotificacionEmailService
    {
        private readonly INotificacionEmailRepository _repository;

        public NotificacionEmailService(INotificacionEmailRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener una notificación por Id
        public async Task<NotificacionEmail?> ObtenerNotificacionPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetNotificacionByIdAsync(id);
        }

        // Caso de uso: Modificar una notificación
        public async Task<string> ModificarNotificacionAsync(NotificacionEmail notificacion)
        {
            if (notificacion.IdNotificacion <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetNotificacionByIdAsync(notificacion.IdNotificacion);
            if (existente == null)
                return "Error: Notificación no encontrada";

            existente.IdResultado = notificacion.IdResultado;
            existente.Asunto = notificacion.Asunto;
            existente.Mensaje = notificacion.Mensaje;
            existente.EstadoEnvio = notificacion.EstadoEnvio;
            existente.Estado = notificacion.Estado;

            await _repository.UpdateNotificacionAsync(existente);
            return "Notificación modificada correctamente";
        }

        // Caso de uso: Obtener todas las notificaciones
        public async Task<IEnumerable<NotificacionEmail>> ObtenerTodasLasNotificacionesAsync()
        {
            return await _repository.GetNotificacionesAsync();
        }

        // Caso de uso: Obtener notificaciones por resultado
        public async Task<IEnumerable<NotificacionEmail>> ObtenerNotificacionesPorResultadoAsync(int idResultado)
        {
            return await _repository.GetNotificacionesByResultadoAsync(idResultado);
        }

        // Caso de uso: Obtener notificaciones por estado de envío
        public async Task<IEnumerable<NotificacionEmail>> ObtenerNotificacionesPorEstadoEnvioAsync(string estadoEnvio)
        {
            return await _repository.GetNotificacionesByEstadoEnvioAsync(estadoEnvio);
        }

        // Caso de uso: Obtener solo notificaciones activas (estado = 1)
        public async Task<IEnumerable<NotificacionEmail>> ObtenerNotificacionesActivasAsync()
        {
            return await _repository.GetNotificacionesByEstadoAsync(true);
        }

        // Caso de uso: Agregar una notificación
        public async Task<string> AgregarNotificacionAsync(NotificacionEmail nuevaNotificacion)
        {
            try
            {
                nuevaNotificacion.Estado = true; // Activa por defecto
                var notificacionInsertada = await _repository.AddNotificacionAsync(nuevaNotificacion);

                if (notificacionInsertada == null || notificacionInsertada.IdNotificacion <= 0)
                    return "Error: No se pudo agregar la notificación";

                return "Notificación agregada correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar notificación (borrado físico)
        public async Task<string> EliminarNotificacionAsync(int id)
        {
            var eliminado = await _repository.DeleteNotificacionAsync(id);

            if (!eliminado)
                return "Error: Notificación no encontrada";

            return "Notificación eliminada correctamente";
        }

        // Caso de uso: Cancelar notificación (soft delete → estado = 0)
        public async Task<string> CancelarNotificacionAsync(int id)
        {
            var notificacion = await _repository.GetNotificacionByIdAsync(id);

            if (notificacion == null)
                return "Error: Notificación no encontrada";

            notificacion.Estado = false; // false = cancelada/inactiva
            await _repository.UpdateNotificacionAsync(notificacion);

            return "Notificación cancelada correctamente";
        }
    }
}
