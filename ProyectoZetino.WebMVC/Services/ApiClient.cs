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
        // 👇 --- 1. Guardamos el Accesor como un campo --- 👇
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
            // Leemos la cookie "justo ahora"
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                // Limpiamos el header si la cookie no existe (ej. después de Logout)
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // --- Roles ---
        public async Task<IEnumerable<RolDto>> GetRolesAsync(string searchTerm = null)
        {
            SetAuthorizationHeader(); // <-- 5. Ponemos el token antes de la llamada

            var url = "api/rol";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";
            }
            return await _httpClient.GetFromJsonAsync<IEnumerable<RolDto>>(url) ?? new List<RolDto>();
        }

        public async Task<RolDto> GetRolAsync(int id)
        {
            SetAuthorizationHeader(); // <-- 5. Ponemos el token antes de la llamada
            return await _httpClient.GetFromJsonAsync<RolDto>($"api/rol/{id}");
        }

        public async Task<bool> CreateRolAsync(RolDto rol)
        {
            SetAuthorizationHeader(); // <-- 5. Ponemos el token antes de la llamada
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
            SetAuthorizationHeader(); // <-- 5. Ponemos el token antes de la llamada
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
            SetAuthorizationHeader(); // <-- 5. Ponemos el token antes de la llamada
            var response = await _httpClient.DeleteAsync($"api/rol/{id}");
            return response.IsSuccessStatusCode;
        }


        // --- Login y Register (No necesitan el token) ---

        public async Task<string?> LoginAsync(string username, string password)
        {
            var payload = new
            {
                Email = username,
                PasswordHash = password,
                Nombre = "Login",
                // ... (resto de tu objeto de login)
                Apellido = "Login",
                Telefono = "00000000",
                FechaNacimiento = DateTime.Now,
                Estado = true,
                IdRol = 0
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", payload);
            //if (!response.IsSuccessStatusCode) return null;

            
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
    }
}