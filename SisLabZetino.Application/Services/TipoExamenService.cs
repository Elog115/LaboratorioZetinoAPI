using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase) para TipoExamen
    public class TipoExamenService
    {
        private readonly ITipoExamenRepository _repository;

        public TipoExamenService(ITipoExamenRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener un tipo de examen por Id
        public async Task<TipoExamen?> ObtenerTipoExamenPorIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetTipoExamenByIdAsync(id);
        }

        // Caso de uso: Modificar tipo de examen
        public async Task<string> ModificarTipoExamenAsync(TipoExamen tipoExamen)
        {
            if (tipoExamen.IdTipoExamen <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetTipoExamenByIdAsync(tipoExamen.IdTipoExamen);
            if (existente == null)
                return "Error: Tipo de examen no encontrado";

            existente.Nombre = tipoExamen.Nombre;
            existente.Descripcion = tipoExamen.Descripcion;
            existente.Precio = tipoExamen.Precio;
            existente.Estado = tipoExamen.Estado;

            await _repository.UpdateTipoExamenAsync(existente);
            return "Tipo de examen modificado correctamente";
        }

        // Caso de uso: Obtener todos los tipos de examen
        public async Task<IEnumerable<TipoExamen>> ObtenerTodosLosTiposExamenAsync()
        {
            return await _repository.GetTiposExamenAsync();
        }

        // Caso de uso: Obtener tipos de examen por estado
        public async Task<IEnumerable<TipoExamen>> ObtenerTiposExamenActivosAsync()
        {
            return await _repository.GetTiposExamenByEstadoAsync(true);
        }

        // Caso de uso: Obtener tipo de examen por nombre
        public async Task<TipoExamen?> ObtenerTipoExamenPorNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null;

            return await _repository.GetTipoExamenByNombreAsync(nombre);
        }

        // Caso de uso: Obtener tipos de examen por rango de precio
        public async Task<IEnumerable<TipoExamen>> ObtenerTiposExamenPorPrecioAsync(decimal precioMin, decimal precioMax)
        {
            return await _repository.GetTiposExamenByPrecioAsync(precioMin, precioMax);
        }

        // Caso de uso: Agregar un tipo de examen
        public async Task<string> AgregarTipoExamenAsync(TipoExamen nuevoTipo)
        {
            try
            {
                nuevoTipo.Estado = true; // Activo por defecto
                var tipoInsertado = await _repository.AddTipoExamenAsync(nuevoTipo);

                if (tipoInsertado == null || tipoInsertado.IdTipoExamen <= 0)
                    return "Error: No se pudo agregar el tipo de examen";

                return "Tipo de examen agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar tipo de examen (borrado físico)
        public async Task<string> EliminarTipoExamenAsync(int id)
        {
            var eliminado = await _repository.DeleteTipoExamenAsync(id);

            if (!eliminado)
                return "Error: Tipo de examen no encontrado";

            return "Tipo de examen eliminado correctamente";
        }

        // Caso de uso: Cancelar tipo de examen (soft delete → estado = 0)
        public async Task<string> CancelarTipoExamenAsync(int id)
        {
            var tipoExamen = await _repository.GetTipoExamenByIdAsync(id);

            if (tipoExamen == null)
                return "Error: Tipo de examen no encontrado";

            tipoExamen.Estado = false; //calcelado/inactivo
            await _repository.UpdateTipoExamenAsync(tipoExamen);

            return "Tipo de examen cancelado correctamente";
        }
    }
}

