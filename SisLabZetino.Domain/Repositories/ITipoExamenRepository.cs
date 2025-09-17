using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de tipos de examen
    public interface ITipoExamenRepository
    {
        //Obtener todos los tipos de examen
        Task<IEnumerable<TipoExamen>> GetTiposExamenAsync();

        //Obtener un tipo de examen por su Id
        Task<TipoExamen> GetTipoExamenByIdAsync(int id);

        //Agregar un nuevo tipo de examen
        Task<TipoExamen> AddTipoExamenAsync(TipoExamen tipoExamen);

        //Actualizar un tipo de examen existente
        Task<TipoExamen> UpdateTipoExamenAsync(TipoExamen tipoExamen);

        //Eliminar un tipo de examen por su Id
        Task<bool> DeleteTipoExamenAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener tipos de examen por nombre
        Task<TipoExamen> GetTipoExamenByNombreAsync(string nombre);

        //Obtener tipos de examen por estado (activo/inactivo)
        Task<IEnumerable<TipoExamen>> GetTiposExamenByEstadoAsync(int estado);

        //Obtener tipos de examen por rango de precio
        Task<IEnumerable<TipoExamen>> GetTiposExamenByPrecioAsync(decimal precioMin, decimal precioMax);
    }
}
