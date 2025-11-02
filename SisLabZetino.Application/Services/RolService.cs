using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Referencias
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    public class RolService
    {
        private readonly IRolRepository _repository;

        public RolService(IRolRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Buscar un rol por Id (Devuelve activos e inactivos)
        public async Task<Rol?> ObtenerRolPorIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var rol = await _repository.GetRolByIdAsync(id);

            // Devuelve el rol sin importar el estado (necesario para el Toggle)
            return rol;
        }

        // Caso de uso: Modificar rol (Actualizado para el Toggle Y Descripcion)
        public async Task<string> ModificarRolAsync(Rol rol)
        {
            if (rol.IdRol <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetRolByIdAsync(rol.IdRol);

            if (existente == null)
                return "Error: Rol no encontrado";

            // Actualizamos los campos que nos manda el MVC
            existente.Nombre = rol.Nombre;
            existente.Estado = rol.Estado;
            existente.Descripcion = rol.Descripcion; // <--- ¡ESTA ES LA LÍNEA NUEVA!

            await _repository.UpdateRolAsync(existente);
            return "Rol modificado correctamente";
        }

        // Caso de uso: Obtener solo roles activos
        public async Task<IEnumerable<Rol>> ObtenerRolesActivosAsync()
        {
            var roles = await _repository.GetRolesAsync();
            return roles.Where(r => r.Estado == true);
        }

        // Caso de uso: Obtener TODOS los roles (activos e inactivos)
        public async Task<IEnumerable<Rol>> ObtenerTodosLosRolesAsync()
        {
            // Simplemente llamamos al método del repositorio que ya trae todo
            return await _repository.GetRolesAsync();
        }

        // Caso de uso: Agregar rol (validar duplicados por nombre)
        public async Task<string> AgregarRolAsync(Rol nuevoRol)
        {
            try
            {
                var roles = await _repository.GetRolesAsync();

                if (roles.Any(r => r.Nombre.ToLower() == nuevoRol.Nombre.ToLower()))
                    return "Error: Ya existe un rol con el mismo nombre";

                // El 'nuevoRol' que viene del ApiClient ya tendrá la Descripcion,
                // así que solo lo pasamos al repositorio.
                nuevoRol.Estado = true;
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

        // Caso de uso: Eliminar rol (Borrado lógico)
        public async Task<string> EliminarRolAsync(int id)
        {
            var existente = await _repository.GetRolByIdAsync(id);
            if (existente == null)
                return "Error: Rol no encontrado";

            existente.Estado = false; // Borrado lógico
            await _repository.UpdateRolAsync(existente);

            return "Rol desactivado correctamente";
        }
    }
}