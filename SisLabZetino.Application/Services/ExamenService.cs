using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase)
    public class ExamenService
    {
        private readonly IExamenRepository _repository;

        public ExamenService(IExamenRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Buscar un examen por Id
        public async Task<Examen?> ObtenerExamenPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetExamenByIdAsync(id);
        }

        // Caso de uso: Modificar examen
        public async Task<string> ModificarExamenAsync(Examen examen)
        {
            if (examen.IdExamen <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetExamenByIdAsync(examen.IdExamen);

            if (existente == null)
                return "Error: Examen no encontrado";

            existente.IdOrdenExamen = examen.IdOrdenExamen;
            existente.IdTipoExamen = examen.IdTipoExamen;
            existente.Descripcion = examen.Descripcion;
            existente.TiempoEstimado = examen.TiempoEstimado;
            existente.Estado = examen.Estado;

            await _repository.UpdateExamenAsync(existente);
            return "Examen modificado correctamente";
        }

        // Caso de uso: Obtener todos los exámenes de una orden
        public async Task<IEnumerable<Examen>> ObtenerExamenesPorOrdenAsync(int idOrdenExamen)
        {
            return await _repository.GetExamenesByOrdenAsync(idOrdenExamen);
        }

        // Caso de uso: Obtener exámenes por tipo
        public async Task<IEnumerable<Examen>> ObtenerExamenesPorTipoAsync(int idTipoExamen)
        {
            return await _repository.GetExamenesByTipoAsync(idTipoExamen);
        }

        // Caso de uso: Obtener solo exámenes activos (estado = 1)
        public async Task<IEnumerable<Examen>> ObtenerExamenesActivosAsync()
        {
            return await _repository.GetExamenesByEstadoAsync(1);
        }

        // Caso de uso: Agregar examen
        public async Task<string> AgregarExamenAsync(Examen nuevoExamen)
        {
            try
            {
                nuevoExamen.Estado = 1; // Activo por defecto
                var examenInsertado = await _repository.AddExamenAsync(nuevoExamen);

                if (examenInsertado == null || examenInsertado.IdExamen <= 0)
                    return "Error: No se pudo agregar el examen";

                return "Examen agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar examen
        public async Task<string> EliminarExamenAsync(int id)
        {
            var eliminado = await _repository.DeleteExamenAsync(id);

            if (!eliminado)
                return "Error: Examen no encontrado";

            return "Examen eliminado correctamente";
        }

        // Caso de uso: Cancelar examen (soft delete → estado = 0)
        public async Task<string> CancelarExamenAsync(int id)
        {
            var examen = await _repository.GetExamenByIdAsync(id);

            if (examen == null)
                return "Error: Examen no encontrado";

            examen.Estado = 0; // 0 = cancelado/inactivo
            await _repository.UpdateExamenAsync(examen);

            return "Examen cancelado correctamente";
        }
    }
}
