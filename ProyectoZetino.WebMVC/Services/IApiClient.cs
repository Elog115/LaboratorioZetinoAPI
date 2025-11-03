
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoZetino.WebMVC.Models;

namespace ProyectoZetino.WebMVC.Services
{
    public interface IApiClient
    {
        // Métodos para Roles
        Task<IEnumerable<RolDto>> GetRolesAsync(string searchTerm = null);
        Task<RolDto> GetRolAsync(int id);
        Task<bool> CreateRolAsync(RolDto rol);
        Task<bool> UpdateRolAsync(int id, RolDto rol);
        Task<bool> DeleteRolAsync(int id);

        // Métodos para autenticación
        Task<string?> LoginAsync(string username, string password);

        // Registro de usuario (web -> API)
        Task<bool> RegisterAsync(RegisterModel model);
    }
}
