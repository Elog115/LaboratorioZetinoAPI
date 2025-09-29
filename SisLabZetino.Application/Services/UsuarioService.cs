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

            if (usuario != null && usuario.Estado == true)
                return usuario; // Solo devolver si está activo

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
            existente.Estado = usuario.Estado;

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
                var usuarios = await _repository.GetUsuariosAsync();

                if (usuarios.Any(p => p.Nombre.ToLower() == nuevoUsuario.Nombre.ToLower()))
                    return "Error: Ya existe un usuario con el mismo nombre";

                nuevoUsuario.Estado = true; //Activo por defecto
                var usuarioinsertado = await _repository.AddUsuarioAsync(nuevoUsuario);

                if (usuarioinsertado == null || usuarioinsertado.IdUsuario <= 0)
                    return "Error: No se pudo agregar el Usuario";

                return "Producto agregado correctamente";
            }

            catch (Exception ex)
            {

                return "Error de servidor:" + ex.Message;
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

        // 🚀 Caso de uso: Validar login
        public async Task<Usuario?> ValidateUsuarioAsync(string correo, string clave)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
                return null;

            var usuario = await _repository.ValidateUsuarioAsync(correo, clave);

            if (usuario != null && usuario.Estado == true) // solo usuarios activos
                return usuario;

            return null;
        }
    }
}
