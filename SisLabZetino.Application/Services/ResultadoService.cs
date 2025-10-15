using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase) para Resultados
    public class ResultadoService
    {
        private readonly IResultadoRepository _repository;

        public ResultadoService(IResultadoRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener un resultado por Id
        public async Task<Resultado?> ObtenerResultadoPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetResultadoByIdAsync(id);
        }

        // Caso de uso: Modificar un resultado
        public async Task<string> ModificarResultadoAsync(Resultado resultado)
        {
            if (resultado.IdResultado <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetResultadoByIdAsync(resultado.IdResultado);
            if (existente == null)
                return "Error: Resultado no encontrado";

            existente.IdExamen = resultado.IdExamen;
            existente.FechaEntrega = resultado.FechaEntrega;
            existente.Observaciones = resultado.Observaciones;
            existente.ArchivoResultado = resultado.ArchivoResultado;
            existente.Estado = resultado.Estado;

            await _repository.UpdateResultadoAsync(existente);
            return "Resultado modificado correctamente";
        }

        // Caso de uso: Obtener todos los resultados
        public async Task<IEnumerable<Resultado>> ObtenerTodosLosResultadosAsync()
        {
            return await _repository.GetResultadosAsync();
        }

        // Caso de uso: Obtener resultados por examen
        public async Task<IEnumerable<Resultado>> ObtenerResultadosPorExamenAsync(int idExamen)
        {
            return await _repository.GetResultadosByExamenAsync(idExamen);
        }

        // Caso de uso: Obtener resultados por fecha de entrega
        public async Task<IEnumerable<Resultado>> ObtenerResultadosPorFechaEntregaAsync(DateTime fechaEntrega)
        {
            return await _repository.GetResultadosByFechaEntregaAsync(fechaEntrega);
        }

        // Caso de uso: Obtener solo resultados activos (estado = 1)
        public async Task<IEnumerable<Resultado>> ObtenerResultadosActivosAsync()
        {
            return await _repository.GetResultadosByEstadoAsync(true);
        }

        // Caso de uso: Agregar un resultado
        public async Task<string> AgregarResultadoAsync(Resultado nuevoResultado)
        {
            try
            {
                nuevoResultado.Estado = true; // Activo por defecto
                var resultadoInsertado = await _repository.AddResultadoAsync(nuevoResultado);

                if (resultadoInsertado == null || resultadoInsertado.IdResultado <= 0)
                    return "Error: No se pudo agregar el resultado";

                return "Resultado agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar resultado (borrado lógico → estado = false)
        public async Task<string> EliminarResultadoAsync(int id)
        {
            var resultado = await _repository.GetResultadoByIdAsync(id);

            if (resultado == null)
                return "Error: Resultado no encontrado";

            resultado.Estado = false; // Marcado como inactivo (borrado lógico)
            await _repository.UpdateResultadoAsync(resultado);

            return "Resultado eliminado correctamente";
        }

        // Caso de uso: Cancelar resultado (soft delete → estado = false)
        public async Task<string> CancelarResultadoAsync(int id)
        {
            var resultado = await _repository.GetResultadoByIdAsync(id);

            if (resultado == null)
                return "Error: Resultado no encontrado";

            resultado.Estado = false; // false = cancelado/inactivo
            await _repository.UpdateResultadoAsync(resultado);

            return "Resultado cancelado correctamente";
        }

    }
}
