using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de citas
    public interface ICitaRepository
    {
        //Obtener todas las citas
        Task<IEnumerable<Cita>> GetCitasAsync();

        //Obtener una cita por su Id
        Task<Cita> GetCitaByIdAsync(int id);

        //Agregar una nueva cita
        Task<Cita> AddCitaAsync(Cita cita);

        //Actualizar una cita existente
        Task<Cita> UpdateCitaAsync(Cita cita);

        //Eliminar una cita por su Id
        Task<bool> EliminarCitaAsync(int id);

        //Métodos adicionales (opcionales):
        //Obtener citas por IdUsuario
        Task<IEnumerable<Cita>> GetCitasByUsuarioAsync(int idUsuario);

        //Obtener citas por estado (ejemplo: activas, canceladas, completadas)
        Task<IEnumerable<Cita>> GetCitasByEstadoAsync(bool estado);
    }
}

