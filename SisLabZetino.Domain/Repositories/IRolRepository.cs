using SisLabZetino.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de roles
    public interface IRolRepository
    {
        //Obtener todos los roles
        Task<IEnumerable<Rol>> GetRolesAsync();

        //Obtener un rol por su Id
        Task<Rol> GetRolByIdAsync(int id);

        //Agregar un nuevo rol
        Task<Rol> AddRolAsync(Rol rol);

        //Actualizar un rol existente
        Task<Rol> UpdateRolAsync(Rol rol);

        //Eliminar un rol por su Id
        Task<bool> DeleteRolAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener roles por estado (ejemplo: activos/inactivos)
        Task<IEnumerable<Rol>> GetRolesByEstadoAsync(int estado);

        //Obtener rol por nombre
        Task<Rol> GetRolByNombreAsync(string nombre);
    }
}

