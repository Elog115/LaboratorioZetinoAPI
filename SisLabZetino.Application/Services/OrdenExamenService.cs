using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Referencias
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase) para las órdenes de examen
    public class OrdenExamenService
    {
        private readonly IOrdenExamenRepository _repository;

        public OrdenExamenService(IOrdenExamenRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener una orden por Id
        public async Task<OrdenExamen?> ObtenerOrdenPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetOrdenExamenByIdAsync(id);
        }

        // Caso de uso: Modificar una orden
        public async Task<string> ModificarOrdenAsync(OrdenExamen orden)
        {
            if (orden.IdOrdenExamen <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetOrdenExamenByIdAsync(orden.IdOrdenExamen);

            if (existente == null)
                return "Error: Orden no encontrada";

            // Actualizar campos
            existente.IdUsuario = orden.IdUsuario;
            existente.IdCita = orden.IdCita;
            existente.FechaSolicitud = orden.FechaSolicitud;
            existente.Estado = orden.Estado;

            await _repository.UpdateOrdenExamenAsync(existente);
            return "Orden modificada correctamente";
        }

        // Caso de uso: Obtener todas las órdenes
        public async Task<IEnumerable<OrdenExamen>> ObtenerTodasLasOrdenesAsync()
        {
            return await _repository.GetOrdenesExamenAsync();
        }

        // Caso de uso: Obtener órdenes por usuario
        public async Task<IEnumerable<OrdenExamen>> ObtenerOrdenesPorUsuarioAsync(int idUsuario)
        {
            return await _repository.GetOrdenesByUsuarioAsync(idUsuario);
        }

        // Caso de uso: Obtener órdenes por cita
        public async Task<IEnumerable<OrdenExamen>> ObtenerOrdenesPorCitaAsync(int idCita)
        {
            return await _repository.GetOrdenesByCitaAsync(idCita);
        }

        // Caso de uso: Obtener órdenes activas (estado = 1)
        public async Task<IEnumerable<OrdenExamen>> ObtenerOrdenesActivasAsync()
        {
            return await _repository.GetOrdenesByEstadoAsync(1);
        }

        // Caso de uso: Obtener órdenes por fecha de solicitud
        public async Task<IEnumerable<OrdenExamen>> ObtenerOrdenesPorFechaSolicitudAsync(DateTime fechaSolicitud)
        {
            return await _repository.GetOrdenesByFechaSolicitudAsync(fechaSolicitud);
        }

        // Caso de uso: Agregar una orden
        public async Task<string> AgregarOrdenAsync(OrdenExamen nuevaOrden)
        {
            try
            {
                nuevaOrden.Estado = 1; // Activa por defecto
                var ordenInsertada = await _repository.AddOrdenExamenAsync(nuevaOrden);

                if (ordenInsertada == null || ordenInsertada.IdOrdenExamen <= 0)
                    return "Error: No se pudo agregar la orden";

                return "Orden agregada correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar orden (borrado físico)
        public async Task<string> EliminarOrdenAsync(int id)
        {
            var eliminado = await _repository.DeleteOrdenExamenAsync(id);

            if (!eliminado)
                return "Error: Orden no encontrada";

            return "Orden eliminada correctamente";
        }

        // Caso de uso: Cancelar orden (soft delete → estado = 0)
        public async Task<string> CancelarOrdenAsync(int id)
        {
            var orden = await _repository.GetOrdenExamenByIdAsync(id);

            if (orden == null)
                return "Error: Orden no encontrada";

            orden.Estado = 0; // 0 = cancelada/inactiva
            await _repository.UpdateOrdenExamenAsync(orden);

            return "Orden cancelada correctamente";
        }
    }
}
