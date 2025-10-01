using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase) para TipoMuestra
    public class TipoMuestraService
    {
        private readonly ITipoMuestraRepository _repository;

        public TipoMuestraService(ITipoMuestraRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener un tipo de muestra por Id
        public async Task<TipoMuestra?> ObtenerTipoMuestraPorIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetTipoMuestraByIdAsync(id);
        }

        // Caso de uso: Modificar tipo de muestra
        public async Task<string> ModificarTipoMuestraAsync(TipoMuestra tipoMuestra)
        {
            if (tipoMuestra.IdTipoMuestra <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetTipoMuestraByIdAsync(tipoMuestra.IdTipoMuestra);
            if (existente == null)
                return "Error: Tipo de muestra no encontrado";

            existente.Nombre = tipoMuestra.Nombre;
            existente.Descripcion = tipoMuestra.Descripcion;
            existente.Estado = tipoMuestra.Estado;

            await _repository.UpdateTipoMuestraAsync(existente);
            return "Tipo de muestra modificado correctamente";
        }

        // Caso de uso: Obtener todos los tipos de muestra
        public async Task<IEnumerable<TipoMuestra>> ObtenerTodosLosTiposMuestraAsync()
        {
            return await _repository.GetTiposMuestraAsync();
        }

        // Caso de uso: Obtener tipos de muestra activos
        public async Task<IEnumerable<TipoMuestra>> ObtenerTiposMuestraActivosAsync()
        {
            return await _repository.GetTiposMuestraByEstadoAsync(true);
        }

        // Caso de uso: Obtener tipo de muestra por nombre
        public async Task<TipoMuestra?> ObtenerTipoMuestraPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null;

            return await _repository.GetTipoMuestraByNombreAsync(nombre);
        }

        // Caso de uso: Agregar un tipo de muestra
        public async Task<string> AgregarTipoMuestraAsync(TipoMuestra nuevoTipo)
        {
            try
            {
                nuevoTipo.Estado = true; // Activo por defecto
                var tipoInsertado = await _repository.AddTipoMuestraAsync(nuevoTipo);

                if (tipoInsertado == null || tipoInsertado.IdTipoMuestra <= 0)
                    return "Error: No se pudo agregar el tipo de muestra";

                return "Tipo de muestra agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar tipo de muestra (borrado físico)
        public async Task<string> EliminarTipoMuestraAsync(int id)
        {
            var eliminado = await _repository.DeleteTipoMuestraAsync(id);

            if (!eliminado)
                return "Error: Tipo de muestra no encontrado";

            return "Tipo de muestra eliminado correctamente";
        }

        // Caso de uso: Cancelar tipo de muestra (soft delete → estado = 0)
        public async Task<string> CancelarTipoMuestraAsync(int id)
        {
            var tipoMuestra = await _repository.GetTipoMuestraByIdAsync(id);

            if (tipoMuestra == null)
                return "Error: Tipo de muestra no encontrado";

            tipoMuestra.Estado = false; // cancelado/inactivo
            await _repository.UpdateTipoMuestraAsync(tipoMuestra);

            return "Tipo de muestra cancelado correctamente";
        }
    }
}
