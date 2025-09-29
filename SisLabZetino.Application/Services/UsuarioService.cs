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
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        // Caso de uso: Buscar un usuario por Id (solo activos)
        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            if (id <= 0)
                return null; // Id no válido

            var usuario = await _repository.GetUsuarioByIdAsync(id);

            if (usuario != null && usuario.Estado == 1) // 1 = activo
                return usuario;

            return null; // No encontrado o inactivo
        }

        // Caso de uso: Modificar usuario
        public async Task<string> ModificarUsuarioAsync(Usuario usuario)
        {
            if (usuario.IdUsuario <= 0)
                return "Error: ID no válido";

            var existente = await _repository.GetUsuarioByIdAsync(usuario.IdUsuario);

            if (existente == null)
                return "Error: Usuario no encontrado";

            existente.Nombre = usuario.Nombre;
            existente.Apellido = usuario.Apellido;
            existente.Correo = usuario.Correo;
            existente.Clave = usuario.Clave;
            existente.FechaNacimiento = usuario.FechaNacimiento;
            existente.Telefono = usuario.Telefono;
            existente.IdRol = usuario.IdRol;
            existente.Estado = usuario.Estado; // Permitir activar/inactivar

            await _repository.UpdateUsuarioAsync(existente);
            return "Usuario modificado correctamente";
        }

        // Caso de uso: Obtener solo usuarios activos
        public async Task<IEnumerable<Usuario>> ObtenerUsuariosActivosAsync()
        {
            var usuarios = await _repository.GetUsuariosAsync();
            return usuarios.Where(u => u.Estado == true);
        }

        // Caso de uso: Agregar usuario (validar duplicados por correo)
        public async Task<string> AgregarUsuarioAsync(Usuario nuevoUsuario)
        {
            try
            {
                var existente = await _repository.GetUsuarioByCorreoAsync(nuevoUsuario.Correo);

                if (existente != null)
                    return "Error: Ya existe un usuario con el mismo correo";

                nuevoUsuario.Estado = 1; // Activo por defecto
                var usuarioInsertado = await _repository.AddUsuarioAsync(nuevoUsuario);

                if (usuarioInsertado == null || usuarioInsertado.IdUsuario <= 0)
                    return "Error: No se pudo agregar el usuario";

                return "Usuario agregado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error de servidor: {ex.Message}";
            }
        }

        // Caso de uso: Eliminar usuario
        public async Task<string> EliminarUsuarioAsync(int id)
        {
            var eliminado = await _repository.DeleteUsuarioAsync(id);

            if (!eliminado)
                return "Error: Usuario no encontrado";

            return "Usuario eliminado correctamente";
        }

        // Caso de uso: Validar usuario (autenticación)
        public async Task<Usuario?> ValidarUsuarioAsync(string correo, string clave)
        {
            return await _repository.ValidateUsuarioAsync(correo, clave);
        }
    }
}
