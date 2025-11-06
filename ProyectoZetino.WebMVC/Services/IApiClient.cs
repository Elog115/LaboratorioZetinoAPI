
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoZetino.WebMVC.Models;

namespace ProyectoZetino.WebMVC.Services
{
    public interface IApiClient
    {
        // --- Métodos para Roles ---
        Task<IEnumerable<RolDto>> GetRolesAsync(string searchTerm = null);
        Task<RolDto> GetRolAsync(int id);
        Task<bool> CreateRolAsync(RolDto rol);
        Task<bool> UpdateRolAsync(int id, RolDto rol);
        Task<bool> DeleteRolAsync(int id);

        // --- Métodos para Autenticación ---
        Task<string?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(RegisterModel model);

        // --- 🔽 Métodos para Usuarios 🔽 ---
        Task<IEnumerable<UsuarioDto>> GetUsuariosAsync(string searchTerm = null);
        Task<UsuarioDto> GetUsuarioAsync(int id);
        Task<bool> CreateUsuarioAsync(UsuarioDto usuario);
        Task<bool> UpdateUsuarioAsync(int id, UsuarioDto usuario);
        Task<bool> DeleteUsuarioAsync(int id);
    }
}
