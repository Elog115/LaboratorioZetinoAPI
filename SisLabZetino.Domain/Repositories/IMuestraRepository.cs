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

        //Actualizar una muestra existente (sirve también para borrado lógico)
        Task<Muestra> UpdateMuestraAsync(Muestra muestra);


    }
}

