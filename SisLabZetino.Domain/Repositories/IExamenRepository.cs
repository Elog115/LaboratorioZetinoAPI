using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    // Contrato del repositorio de exámenes
    public interface IExamenRepository
    {
        // Obtener todos los exámenes
        Task<IEnumerable<Examen>> GetExamenesAsync();

        // Obtener un examen por su Id
        Task<Examen> GetExamenByIdAsync(int id);

        // Agregar un nuevo examen
        Task<Examen> AddExamenAsync(Examen examen);

        // Actualizar un examen existente (sirve también para borrado lógico)
        Task<Examen> UpdateExamenAsync(Examen examen);
    }
}
