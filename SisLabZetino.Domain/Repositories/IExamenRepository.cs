using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de exámenes
    public interface IExamenRepository
    {
        //Obtener todos los exámenes
        Task<IEnumerable<Examen>> GetExamenesAsync();

        //Obtener un examen por su Id
        Task<Examen> GetExamenByIdAsync(int id);

        //Agregar un nuevo examen
        Task<Examen> AddExamenAsync(Examen examen);

        //Actualizar un examen existente
        Task<Examen> UpdateExamenAsync(Examen examen);

        //Eliminar un examen por su Id
        Task<bool> DeleteExamenAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener exámenes por IdOrdenExamen
        Task<IEnumerable<Examen>> GetExamenesByOrdenAsync(int idOrdenExamen);

        //Obtener exámenes por IdTipoExamen
        Task<IEnumerable<Examen>> GetExamenesByTipoAsync(int idTipoExamen);

        //Obtener exámenes por estado
        Task<IEnumerable<Examen>> GetExamenesByEstadoAsync(int estado);
    }
}
