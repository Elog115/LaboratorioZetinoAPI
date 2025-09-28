using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de usuarios del sistema
    public interface IUsuarioRepository
    {
        //Obtener todos los usuarios
        Task<IEnumerable<Usuario>> GetUsuariosAsync();

        //Obtener un usuario por su Id
        Task<Usuario> GetUsuarioByIdAsync(int id);

        //Agregar un nuevo usuario
        Task<Usuario> AddUsuarioAsync(Usuario usuario);

        //Actualizar un usuario existente
        Task<Usuario> UpdateUsuarioAsync(Usuario usuario);

        //Eliminar un usuario por su Id
        Task<bool> DeleteUsuarioAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener usuarios por IdRol
        Task<IEnumerable<Usuario>> GetUsuariosByRolAsync(int idRol);

        //Obtener usuarios por estado (activo/inactivo)
        Task<IEnumerable<Usuario>> GetUsuariosByEstadoAsync(int estado);

        //Obtener usuario por correo
        Task<Usuario> GetUsuarioByCorreoAsync(string correo);

        //Autenticación: validar correo y clave
        Task<Usuario> ValidateUsuarioAsync(string correo, string clave);
    }
}
