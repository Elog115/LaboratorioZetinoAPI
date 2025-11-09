using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoZetino.WebMVC.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

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



        public async Task<bool> UpdateUsuarioAsync(int id, UsuarioDto usuario)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/auth/usuarios/{id}", usuario);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/auth/usuarios/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<UsuarioDto?> GetUsuarioByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/auth/usuarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UsuarioDto>();
            }
            return null;
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
                fechaHora = cita.FechaHora,     // DateTime? se serializa en ISO
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



    }
}
