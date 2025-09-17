using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de resultados
    public interface IResultadoRepository
    {
        //Obtener todos los resultados
        Task<IEnumerable<Resultado>> GetResultadosAsync();

        //Obtener un resultado por su Id
        Task<Resultado> GetResultadoByIdAsync(int id);

        //Agregar un nuevo resultado
        Task<Resultado> AddResultadoAsync(Resultado resultado);

        //Actualizar un resultado existente
        Task<Resultado> UpdateResultadoAsync(Resultado resultado);

        //Eliminar un resultado por su Id
        Task<bool> DeleteResultadoAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener resultados por IdExamen
        Task<IEnumerable<Resultado>> GetResultadosByExamenAsync(int idExamen);

        //Obtener resultados por fecha de entrega
        Task<IEnumerable<Resultado>> GetResultadosByFechaEntregaAsync(DateTime fechaEntrega);

        //Obtener resultados por estado
        Task<IEnumerable<Resultado>> GetResultadosByEstadoAsync(int estado);
    }
}
