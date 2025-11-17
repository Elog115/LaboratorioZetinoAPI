using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProyectoZetino.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private class LoginResponse
        {
            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // --- Roles ---
        public async Task<IEnumerable<RolDto>> GetRolesAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();
            var url = "api/rol";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<RolDto>>(url) ?? new List<RolDto>();
        }

        public async Task<RolDto> GetRolAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<RolDto>($"api/rol/{id}");
        }

        public async Task<bool> CreateRolAsync(RolDto rol)
        {
            SetAuthorizationHeader();
            var payload = new
            {
                rol.Nombre,
                rol.Estado,
                rol.Descripcion
            };
            var response = await _httpClient.PostAsJsonAsync("api/rol", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRolAsync(int id, RolDto rol)
        {
            SetAuthorizationHeader();
            var payload = new
            {
                rol.IdRol,
                rol.Nombre,
                rol.Estado,
                rol.Descripcion
            };
            var response = await _httpClient.PutAsJsonAsync($"api/rol/{id}", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRolAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/rol/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Login y Register ---
        public async Task<string?> LoginAsync(string username, string password)
        {
            var payload = new
            {
                Email = username,
                PasswordHash = password,
                Nombre = "Login",
                Apellido = "Login",
                Telefono = "00000000",
                FechaNacimiento = DateTime.Now,
                Estado = true,
                IdRol = 0
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", payload);
            if (!response.IsSuccessStatusCode)
                return null;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResponse?.Token;
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var payload = new
            {
                Nombre = model.Nombre,
                Email = model.Email,
                PasswordHash = model.Password,
                IdRol = 2,
                Apellido = model.Apellido,
                Telefono = model.Telefono,
                FechaNacimiento = model.FechaNacimiento,
                Estado = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", payload);
            return response.IsSuccessStatusCode;
        }

        // --- Usuarios ---
        public async Task<IEnumerable<UsuarioDto>> GetUsuariosAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();
            var url = "api/auth/usuarios";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?searchTerm={System.Net.WebUtility.UrlEncode(searchTerm)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<UsuarioDto>>(url) ?? new List<UsuarioDto>();
        }

        public async Task<UsuarioDto> GetUsuarioAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<UsuarioDto>($"api/auth/usuarios/{id}");
        }

        public async Task<bool> CreateUsuarioAsync(UsuarioDto usuario)
        {
            SetAuthorizationHeader();

            // Mapeo manual al formato que espera la API
            var nuevoUsuario = new
            {
                idUsuario = usuario.IdUsuario,
                idRol = usuario.IdRol,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                telefono = usuario.Telefono,
                email = usuario.Email,
                fechaNacimiento = usuario.FechaNacimiento,
                passwordHash = usuario.Password,  // 🔑 La API espera "PasswordHash"
                estado = usuario.Estado
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/usuarios", nuevoUsuario);
            return response.IsSuccessStatusCode;
        }

        // 🔹 Obtener usuario por ID
        public async Task<UsuarioDto?> GetUsuarioByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/auth/usuarios/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UsuarioDto>(content);
        }

        // 🔹 Eliminar usuario
        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/auth/usuarios/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateUsuarioAsync(int id, UsuarioDto usuario)
        {
            SetAuthorizationHeader();

            // Crear el objeto que coincida con lo que la API espera (entidad Usuario)
            var usuarioActualizado = new
            {
                IdUsuario = usuario.IdUsuario,
                IdRol = usuario.IdRol,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono,
                Email = usuario.Email,
                FechaNacimiento = usuario.FechaNacimiento,
                PasswordHash = usuario.Password, // ⚠️ IMPORTANTE: la API espera PasswordHash
                Estado = true
            };

            var response = await _httpClient.PutAsJsonAsync($"api/auth/usuarios/{id}", usuarioActualizado);
            return response.IsSuccessStatusCode;
        }



        // --- Citas ---
        public async Task<IEnumerable<CitaDto>> GetCitasAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();
            var url = "api/cita";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<CitaDto>>(url) ?? new List<CitaDto>();
        }

        public async Task<CitaDto> GetCitaAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<CitaDto>($"api/cita/{id}");
        }

        public async Task<bool> CreateCitaAsync(CitaDto cita)
        {
            SetAuthorizationHeader();

            // Mapeo al formato que espera la API
            var payload = new
            {
                idCita = cita.IdCita,
                idUsuario = cita.IdUsuario,
                fechaHora = cita.FechaHora,      // DateTime? se serializa en ISO
                descripcion = cita.Descripcion,
                estado = cita.Estado
            };

            var response = await _httpClient.PostAsJsonAsync("api/cita", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCitaAsync(int id, CitaDto cita)
        {
            SetAuthorizationHeader();

            var payload = new
            {
                idCita = cita.IdCita,
                idUsuario = cita.IdUsuario,
                fechaHora = cita.FechaHora,
                descripcion = cita.Descripcion,
                estado = cita.Estado
            };

            var response = await _httpClient.PutAsJsonAsync($"api/cita/{id}", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCitaAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/cita/{id}");
            return response.IsSuccessStatusCode;
        }

        // ✅ Exámenes
        public async Task<IEnumerable<ExamenDto>> GetExamenesAsync(string searchTerm = null)
        {
            var url = "api/examen";

            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?searchTerm={searchTerm}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<ExamenDto>>(url)
                   ?? new List<ExamenDto>();
        }

        public async Task<ExamenDto?> GetExamenByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ExamenDto>($"api/examen/{id}");
        }

        public async Task<bool> CreateExamenAsync(ExamenDto examen)
        {
            var response = await _httpClient.PostAsJsonAsync("api/examen", examen);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateExamenAsync(int id, ExamenDto examen)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/examen/{id}", examen);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteExamenAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/examen/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<ExamenDto> GetExamenAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ExamenDto>($"api/examen/{id}");
        }



        // ✅ Orden de Examen

        public async Task<IEnumerable<OrdenExamenDto>> GetOrdenesExamenAsync(string searchTerm = null)
        {
            var url = "api/orden-examen";

            // Tu API NO TIENE búsqueda, así que ignoramos searchTerm
            return await _httpClient.GetFromJsonAsync<IEnumerable<OrdenExamenDto>>(url);
        }

        public async Task<OrdenExamenDto> GetOrdenExamenAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<OrdenExamenDto>($"api/orden-examen/{id}");
        }

        public async Task<OrdenExamenDto?> GetOrdenExamenByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<OrdenExamenDto>($"api/orden-examen/{id}");
        }

        public async Task<bool> CreateOrdenExamenAsync(OrdenExamenDto orden)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orden-examen", orden);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrdenExamenAsync(int id, OrdenExamenDto orden)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/orden-examen/{id}", orden);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrdenExamenAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/orden-examen/{id}");
            return response.IsSuccessStatusCode;
        }


        // ✅ Tipo de Examen

        public async Task<IEnumerable<TipoExamenDto>> GetTiposExamenAsync(string searchTerm = null)
        {
            // Tu API NO usa searchTerm, así que lo ignoramos
            return await _httpClient.GetFromJsonAsync<IEnumerable<TipoExamenDto>>("api/tipo-examen");
        }

        public async Task<TipoExamenDto> GetTipoExamenAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TipoExamenDto>($"api/tipo-examen/{id}");
        }

        public async Task<TipoExamenDto?> GetTipoExamenByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TipoExamenDto>($"api/tipo-examen/{id}");
        }

        public async Task<bool> CreateTipoExamenAsync(TipoExamenDto tipo)
        {
            var response = await _httpClient.PostAsJsonAsync("api/tipo-examen", tipo);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTipoExamenAsync(int id, TipoExamenDto tipo)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/tipo-examen/{id}", tipo);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTipoExamenAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/tipo-examen/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Tipos de Muestra ---

        public async Task<IEnumerable<TipoMuestraDto>> GetTiposMuestraAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();
            var url = "api/tipomuestra"; // <- Asumiendo que esta es la ruta de tu API
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<TipoMuestraDto>>(url) ?? new List<TipoMuestraDto>();
        }

        public async Task<TipoMuestraDto> GetTipoMuestraAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<TipoMuestraDto>($"api/tipomuestra/{id}");
        }

        public async Task<bool> CreateTipoMuestraAsync(TipoMuestraDto tipoMuestra)
        {
            SetAuthorizationHeader();

            // Replicamos el patrón de 'Rol': objeto anónimo con propiedades PascalCase
            var payload = new
            {
                tipoMuestra.Nombre,
                tipoMuestra.Descripcion,
                tipoMuestra.Estado
            };
            var response = await _httpClient.PostAsJsonAsync("api/tipomuestra", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTipoMuestraAsync(int id, TipoMuestraDto tipoMuestra)
        {
            SetAuthorizationHeader();

            // Replicamos el patrón de 'Rol': objeto anónimo con propiedades PascalCase
            var payload = new
            {
                tipoMuestra.IdTipoMuestra,
                tipoMuestra.Nombre,
                tipoMuestra.Descripcion,
                tipoMuestra.Estado
            };
            var response = await _httpClient.PutAsJsonAsync($"api/tipomuestra/{id}", payload);
            return response.IsSuccessStatusCode;
        }
        // --- Muestras ---

        public async Task<IEnumerable<MuestraDto>> GetMuestrasAsync()
        {
            SetAuthorizationHeader();
            // La API no acepta 'searchTerm', por eso no lo enviamos.
            var url = "api/muestra";
            return await _httpClient.GetFromJsonAsync<IEnumerable<MuestraDto>>(url) ?? new List<MuestraDto>();
        }

        public async Task<MuestraDto> GetMuestraAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<MuestraDto>($"api/muestra/{id}");
        }

        public async Task<string> CreateMuestraAsync(MuestraDto muestra)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/muestra", muestra);

            // Leemos la respuesta como string, ya que tu API devuelve "Ok(resultado)"
            var responseString = await response.Content.ReadAsStringAsync();

            // Si la API falló (ej. 500) o no fue exitoso, devolvemos el error
            if (!response.IsSuccessStatusCode)
            {
                // Limpiamos comillas si es un JSON de error
                return responseString.Trim('"');
            }

            // Si fue exitoso, devolvemos el string (ej. "Muestra agregada")
            return responseString.Trim('"');
        }

        public async Task<string> UpdateMuestraAsync(int id, MuestraDto muestra)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/muestra/{id}", muestra);

            // Leemos la respuesta como string
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return responseString.Trim('"');
            }

            return responseString.Trim('"');
        }
        // --- Notificaciones Email ---

        public async Task<IEnumerable<NotificacionEmailDto>> GetNotificacionesEmailAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();

            // Ruta con guion, como en tu API
            var url = "api/notificacion-email";

            // Tu API GET no acepta 'searchTerm', así que ignoramos ese parámetro.
            // El filtro lo haremos por JS en el cliente.

            return await _httpClient.GetFromJsonAsync<IEnumerable<NotificacionEmailDto>>(url) ?? new List<NotificacionEmailDto>();
        }

        public async Task<NotificacionEmailDto> GetNotificacionEmailAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<NotificacionEmailDto>($"api/notificacion-email/{id}");
        }

        public async Task<string> UpdateNotificacionEmailAsync(int id, NotificacionEmailDto notificacion)
        {
            SetAuthorizationHeader();

            // Tu API devuelve un string, así que replicamos el patrón de Muestra
            var response = await _httpClient.PutAsJsonAsync($"api/notificacion-email/{id}", notificacion);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return responseString.Trim('"');
            }
            return responseString.Trim('"');
        }
        // --- NUEVOS MÉTODOS PARA RESULTADOS ---

        public async Task<IEnumerable<ResultadoDto>> GetResultadosAsync(string searchTerm = null)
        {
            SetAuthorizationHeader();
            var url = "api/resultado"; // Asumiendo ruta de API
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<ResultadoDto>>(url) ?? new List<ResultadoDto>();
        }

        public async Task<ResultadoDto> GetResultadoAsync(int id)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ResultadoDto>($"api/resultado/{id}");
        }

        public async Task<bool> CreateResultadoAsync(ResultadoDto resultado)
        {
            SetAuthorizationHeader();

            // Usamos un payload anónimo para que coincida con lo que la API espera
            var payload = new
            {
                resultado.IdExamen,
                resultado.FechaEntrega,
                resultado.Observaciones,
                resultado.ArchivoResultado,
                resultado.Estado
            };
            var response = await _httpClient.PostAsJsonAsync("api/resultado", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateResultadoAsync(int id, ResultadoDto resultado)
        {
            SetAuthorizationHeader();

            // Usamos un payload anónimo para que coincida con lo que la API espera
            var payload = new
            {
                resultado.IdResultado,
                resultado.IdExamen,
                resultado.FechaEntrega,
                resultado.Observaciones,
                resultado.ArchivoResultado,
                resultado.Estado
            };
            var response = await _httpClient.PutAsJsonAsync($"api/resultado/{id}", payload);
            return response.IsSuccessStatusCode;
        }
    }
}