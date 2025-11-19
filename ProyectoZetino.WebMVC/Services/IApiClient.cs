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
        Task<UsuarioDto?> GetUsuarioByIdAsync(int id);
        Task<bool> ToggleEstadoUsuarioAsync(int id);

        // Citas
        Task<IEnumerable<CitaDto>> GetCitasAsync(string searchTerm = null);
        Task<CitaDto> GetCitaAsync(int id);
        Task<bool> CreateCitaAsync(CitaDto cita);
        Task<bool> UpdateCitaAsync(int id, CitaDto cita);
        Task<bool> DeleteCitaAsync(int id);

        // --- Exámenes ---
        Task<IEnumerable<ExamenDto>> GetExamenesAsync(string searchTerm = null);
        Task<ExamenDto> GetExamenAsync(int id);
        Task<bool> CreateExamenAsync(ExamenDto examen);
        Task<bool> UpdateExamenAsync(int id, ExamenDto examen);
        Task<bool> DeleteExamenAsync(int id);
        Task<ExamenDto?> GetExamenByIdAsync(int id);

        // --- Orden de Examen ---
        Task<IEnumerable<OrdenExamenDto>> GetOrdenesExamenAsync(string searchTerm = null);
        Task<OrdenExamenDto> GetOrdenExamenAsync(int id);
        Task<bool> CreateOrdenExamenAsync(OrdenExamenDto orden);
        Task<bool> UpdateOrdenExamenAsync(int id, OrdenExamenDto orden);
        Task<bool> DeleteOrdenExamenAsync(int id);
        Task<OrdenExamenDto?> GetOrdenExamenByIdAsync(int id);

        // --- Tipo de Examen ---
        Task<IEnumerable<TipoExamenDto>> GetTiposExamenAsync(string searchTerm = null);
        Task<TipoExamenDto> GetTipoExamenAsync(int id);
        Task<bool> CreateTipoExamenAsync(TipoExamenDto tipo);
        Task<bool> UpdateTipoExamenAsync(int id, TipoExamenDto tipo);
        Task<bool> DeleteTipoExamenAsync(int id);
        Task<TipoExamenDto?> GetTipoExamenByIdAsync(int id);

        // --- Tipos de Muestra ---
        Task<IEnumerable<TipoMuestraDto>> GetTiposMuestraAsync(string searchTerm = null);
        Task<TipoMuestraDto> GetTipoMuestraAsync(int id);
        Task<bool> CreateTipoMuestraAsync(TipoMuestraDto tipoMuestra);
        Task<bool> UpdateTipoMuestraAsync(int id, TipoMuestraDto tipoMuestra);

        // --- Muestras ---
        Task<IEnumerable<MuestraDto>> GetMuestrasAsync();
        Task<MuestraDto> GetMuestraAsync(int id);
        Task<string> CreateMuestraAsync(MuestraDto muestra); // Devuelve string
        Task<string> UpdateMuestraAsync(int id, MuestraDto muestra); // Devuelve string

        // --- Notificaciones Email ---
        Task<IEnumerable<NotificacionEmailDto>> GetNotificacionesEmailAsync(string searchTerm = null);
        Task<NotificacionEmailDto> GetNotificacionEmailAsync(int id);
        Task<string> UpdateNotificacionEmailAsync(int id, NotificacionEmailDto notificacion); // Devuelve string
        // --- Resultados ---
        Task<IEnumerable<ResultadoDto>> GetResultadosAsync(string searchTerm = null);
        Task<ResultadoDto> GetResultadoAsync(int id);
        Task<bool> CreateResultadoAsync(ResultadoDto resultado);
        Task<bool> UpdateResultadoAsync(int id, ResultadoDto resultado);
        // Nota: No hay DeleteResultadoAsync porque usaremos ToggleEstado (que es un Update)
    }
}
