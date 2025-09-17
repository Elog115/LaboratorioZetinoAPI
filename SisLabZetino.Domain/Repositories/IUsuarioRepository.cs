using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisLabZetino.Domain.Repositories
{
    //Contrato del repositorio de usuarios del sistema
    public interface IUsuarioSistemaRepository
    {
        //Obtener todos los usuarios
        Task<IEnumerable<UsuarioSistema>> GetUsuariosAsync();

        //Obtener un usuario por su Id
        Task<UsuarioSistema> GetUsuarioByIdAsync(int id);

        //Agregar un nuevo usuario
        Task<UsuarioSistema> AddUsuarioAsync(UsuarioSistema usuario);

        //Actualizar un usuario existente
        Task<UsuarioSistema> UpdateUsuarioAsync(UsuarioSistema usuario);

        //Eliminar un usuario por su Id
        Task<bool> DeleteUsuarioAsync(int id);

        //Métodos adicionales (opcionales)
        //Obtener usuarios por IdRol
        Task<IEnumerable<UsuarioSistema>> GetUsuariosByRolAsync(int idRol);

        //Obtener usuarios por estado (activo/inactivo)
        Task<IEnumerable<UsuarioSistema>> GetUsuariosByEstadoAsync(int estado);

        //Obtener usuario por correo
        Task<UsuarioSistema> GetUsuarioByCorreoAsync(string correo);

        //Autenticación: validar correo y clave
        Task<UsuarioSistema> ValidateUsuarioAsync(string correo, string clave);
    }
}
