using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de tipos de muestra
    public interface ITipoMuestraRepository
    {
        //Obtener todos los tipos de muestra
        Task<IEnumerable<TipoMuestra>> GetTiposMuestraAsync();

        //Obtener un tipo de muestra por su Id
        Task<TipoMuestra> GetTipoMuestraByIdAsync(int id);

        //Agregar un nuevo tipo de muestra
        Task<TipoMuestra> AddTipoMuestraAsync(TipoMuestra tipoMuestra);

        //Actualizar un tipo de muestra existente
        Task<TipoMuestra> UpdateTipoMuestraAsync(TipoMuestra tipoMuestra);

        //Eliminar un tipo de muestra por su Id
        Task<bool> DeleteTipoMuestraAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener tipos de muestra por nombre
        Task<TipoMuestra> GetTipoMuestraByNombreAsync(string nombre);

        //Obtener tipos de muestra por estado (activo/inactivo)
        Task<IEnumerable<TipoMuestra>> GetTiposMuestraByEstadoAsync(bool estado);
    }
}
