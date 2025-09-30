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
    public class MuestraService
    {
        private readonly IMuestraRepository _repository;

        public MuestraService(IMuestraRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Obtener una muestra por Id
        public async Task<Muestra?> ObtenerMuestraPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            return await _repository.GetMuestraByIdAsync(id);
        }

        // Caso de uso: Modificar muestra
        public async Task<string> ModificarMuestraAsync(Muestra muestra)
        {
            if (muestra.IdMuestra <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetMuestraByIdAsync(muestra.IdMuestra);

            if (existente == null)
                return "Error: Muestra no encontrada";

            existente.IdOrdenExamen = muestra.IdOrdenExamen;
            existente.IdTipoMuestra = muestra.IdTipoMuestra;
            existente.Estado = muestra.Estado;
            existente.FechaRecepcion = muestra.FechaRecepcion;

            await _repository.UpdateMuestraAsync(existente);
            return "Muestra modificada correctamente";
        }

        

        // Caso de uso: Obtener solo muestras activas (estado = 1)
        public async Task<IEnumerable<Muestra>> ObtenerMuestrasActivasAsync()
        {
            var muestras = await _repository.GetMuestrasAsync();
            return muestras.Where(m => m.Estado == true); 
        }

        // Caso de uso: Agregar una muestra
        public async Task<string> AgregarMuestraAsync(Muestra nuevaMuestra)
        {
            try
            {
                nuevaMuestra.Estado = true; // Activa por defecto
                var muestraInsertada = await _repository.AddMuestraAsync(nuevaMuestra);

                if (muestraInsertada == null || muestraInsertada.IdMuestra <= 0)
                    return "Error: No se pudo agregar la muestra";

                return "Muestra agregada correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar Muestra (borrado lógico)
        public async Task<string> EliminarMuestraAsync(int id)
        {
            if (id <= 0)
                return "Error: ID no válido";

            var muestra = await _repository.GetMuestraByIdAsync(id);

            if (muestra == null)
                return "Error: Muestra no encontrado";

            // 🔹 Borrado lógico → marcar como inactivo
            muestra.Estado = false;

            await _repository.UpdateMuestraAsync(muestra);

            return "Muestra eliminado correctamente (inactivo)";
        }

    }
}

