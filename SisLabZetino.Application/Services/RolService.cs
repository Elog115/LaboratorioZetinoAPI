using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Referencias
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    // Algoritmos con lógica de negocio (UseCase)
    public class RolService
    {
        private readonly IRolRepository _repository;

        public RolService(IRolRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Buscar un rol por Id (solo activos)
        public async Task<Rol?> ObtenerRolPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            var rol = await _repository.GetRolByIdAsync(id);

            if (rol != null && rol.Estado == true)
                // 1 = activo
                return rol;

            return null; // No encontrado o inactivo
        }

        // Caso de uso: Modificar rol
        public async Task<string> ModificarRolAsync(Rol rol)
        {
            if (rol.IdRol <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetRolByIdAsync(rol.IdRol);

            if (existente == null)
                return "Error: Rol no encontrado";

            existente.Nombre = rol.Nombre;
            existente.Estado = rol.Estado;

            await _repository.UpdateRolAsync(existente);
            return "Rol modificado correctamente";
        }

        // Caso de uso: Obtener solo roles activos
        public async Task<IEnumerable<Rol>> ObtenerRolesActivosAsync()
        {
            var roles = await _repository.GetRolesAsync();
            return roles.Where(r => r.Estado == true);
        }

        // Caso de uso: Agregar rol (validar duplicados por nombre)
        public async Task<string> AgregarRolAsync(Rol nuevoRol)
        {
            try
            {
                var roles = await _repository.GetRolesAsync();

                // Validar duplicados por nombre (ignorando mayúsculas/minúsculas)
                if (roles.Any(r => r.Nombre.ToLower() == nuevoRol.Nombre.ToLower()))
                    return "Error: Ya existe un rol con el mismo nombre";

                nuevoRol.Estado = true; // Activo por defecto (bool)
                var rolInsertado = await _repository.AddRolAsync(nuevoRol);

                if (rolInsertado == null || rolInsertado.IdRol <= 0)
                    return "Error: No se pudo agregar el rol";

                return "Rol agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }


        // Caso de uso: Eliminar rol
        public async Task<string> EliminarRolAsync(int id)
        {
            var eliminado = await _repository.DeleteRolAsync(id);

            if (!eliminado)
                return "Error: Rol no encontrado";

            return "Rol eliminado correctamente";
        }
    }
}

