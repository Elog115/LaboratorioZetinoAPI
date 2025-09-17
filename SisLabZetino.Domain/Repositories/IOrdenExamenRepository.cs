using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de órdenes de examen
    public interface IOrdenExamenRepository
    {
        //Obtener todas las órdenes de examen
        Task<IEnumerable<OrdenExamen>> GetOrdenesExamenAsync();

        //Obtener una orden de examen por su Id
        Task<OrdenExamen> GetOrdenExamenByIdAsync(int id);

        //Agregar una nueva orden de examen
        Task<OrdenExamen> AddOrdenExamenAsync(OrdenExamen ordenExamen);

        //Actualizar una orden de examen existente
        Task<OrdenExamen> UpdateOrdenExamenAsync(OrdenExamen ordenExamen);

        //Eliminar una orden de examen por su Id
        Task<bool> DeleteOrdenExamenAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener órdenes de examen por IdUsuario
        Task<IEnumerable<OrdenExamen>> GetOrdenesByUsuarioAsync(int idUsuario);

        //Obtener órdenes de examen por IdCita
        Task<IEnumerable<OrdenExamen>> GetOrdenesByCitaAsync(int idCita);

        //Obtener órdenes de examen por fecha de solicitud
        Task<IEnumerable<OrdenExamen>> GetOrdenesByFechaSolicitudAsync(DateTime fechaSolicitud);

        //Obtener órdenes de examen por estado
        Task<IEnumerable<OrdenExamen>> GetOrdenesByEstadoAsync(int estado);
    }
}
