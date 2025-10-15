using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Referencias
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase)
    public class CitaService
    {
        private readonly ICitaRepository _repository;

        public CitaService(ICitaRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Buscar una cita por Id
        public async Task<Cita?> ObtenerCitaPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetCitaByIdAsync(id);
        }

        // Caso de uso: Modificar cita
        public async Task<string> ModificarCitaAsync(Cita cita)
        {
            if (cita.IdCita <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetCitaByIdAsync(cita.IdCita);

            if (existente == null)
                return "Error: Cita no encontrada";

            existente.IdUsuario = cita.IdUsuario;
            existente.FechaHora = cita.FechaHora;
            existente.Descripcion = cita.Descripcion;
            existente.Estado = cita.Estado;

            await _repository.UpdateCitaAsync(existente);
            return "Cita modificada correctamente";
        }

        // Caso de uso: Obtener todas las citas de un usuario
        public async Task<IEnumerable<Cita>> ObtenerCitasPorUsuarioAsync(int idUsuario)
        {
            return await _repository.GetCitasByUsuarioAsync(idUsuario);
        }

        // Caso de uso: Obtener solo citas activas (estado = true)
        public async Task<IEnumerable<Cita>> ObtenerCitasActivasAsync()
        {
            return await _repository.GetCitasByEstadoAsync(true);
        }

        // Caso de uso: Agregar una cita (validar que el usuario no tenga otra en la misma fecha y hora)
        public async Task<string> AgregarCitaAsync(Cita nuevaCita)
        {
            try
            {
                var citasUsuario = await _repository.GetCitasByUsuarioAsync(nuevaCita.IdUsuario);

                if (citasUsuario.Any(c => c.FechaHora == nuevaCita.FechaHora))
                    return "Error: El usuario ya tiene una cita en la misma fecha y hora";

                nuevaCita.Estado = true; // Activa por defecto
                var citaInsertada = await _repository.AddCitaAsync(nuevaCita);

                if (citaInsertada == null || citaInsertada.IdCita <= 0)
                    return "Error: No se pudo agregar la cita";

                return "Cita agregada correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar cita (borrado lógico → estado = false)
        public async Task<string> EliminarCitaAsync(int id)
        {
            var cita = await _repository.GetCitaByIdAsync(id);

            if (cita == null)
                return "Error: Cita no encontrada";

            cita.Estado = false; // Desactivar cita en lugar de eliminar físicamente
            await _repository.UpdateCitaAsync(cita);

            return "Cita eliminada correctamente";
        }

        // Caso de uso: Cancelar una cita (soft delete → estado = 0)
        public async Task<string> CancelarCitaAsync(int id)
        {
            var cita = await _repository.GetCitaByIdAsync(id);

            if (cita == null)
                return "Error: Cita no encontrada";

            cita.Estado = false; // false = cancelada
            await _repository.UpdateCitaAsync(cita);

            return "Cita cancelada correctamente";
        }
    }
}
