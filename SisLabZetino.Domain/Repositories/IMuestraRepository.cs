using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de muestras
    public interface IMuestraRepository
    {
        //Obtener todas las muestras
        Task<IEnumerable<Muestra>> GetMuestrasAsync();

        //Obtener una muestra por su Id
        Task<Muestra> GetMuestraByIdAsync(int id);

        //Agregar una nueva muestra
        Task<Muestra> AddMuestraAsync(Muestra muestra);

        //Actualizar una muestra existente
        Task<Muestra> UpdateMuestraAsync(Muestra muestra);

        //Eliminar una muestra por su Id
        Task<bool> DeleteMuestraAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener muestras por IdOrdenExamen
        Task<IEnumerable<Muestra>> GetMuestrasByOrdenAsync(int idOrdenExamen);

        //Obtener muestras por IdTipoMuestra
        Task<IEnumerable<Muestra>> GetMuestrasByTipoAsync(int idTipoMuestra);

        //Obtener muestras por estado
        Task<IEnumerable<Muestra>> GetMuestrasByEstadoAsync(int estado);

        //Obtener muestras por fecha de recepción
        Task<IEnumerable<Muestra>> GetMuestrasByFechaRecepcionAsync(DateTime fechaRecepcion);
    }
}

