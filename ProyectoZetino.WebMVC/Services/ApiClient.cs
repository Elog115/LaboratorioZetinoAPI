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

        private class LoginResponse
        {
            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }

        // Constructor con JWT (Esto está bien)
        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            var token = httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // --- Roles ---

        // ----- 👇 CÓDIGO MODIFICADO PARA EL BUSCADOR 👇 -----
        public async Task<IEnumerable<RolDto>> GetRolesAsync(string searchTerm = null)
        {
            // 1. Construimos la URL base
            var url = "api/rol";

            // 2. Si hay un término de búsqueda, lo añadimos como query string
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Asegúrate de que tu API pueda recibir un parámetro "search"
                // El Url.Encode es una buena práctica por si buscan "Rol con espacio"
                url += $"?search={System.Net.WebUtility.UrlEncode(searchTerm)}";
            }

            // 3. Hacemos la llamada a la URL (con o sin el ?search=)
            return await _httpClient.GetFromJsonAsync<IEnumerable<RolDto>>(url) ?? new List<RolDto>();
        }
        // ----- 👆 FIN DE LA MODIFICACIÓN 👆 -----

        public async Task<RolDto> GetRolAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<RolDto>($"api/rol/{id}");
        }

        public async Task<bool> CreateRolAsync(RolDto rol)
        {
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
            var response = await _httpClient.DeleteAsync($"api/rol/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Login y Register (Sin cambios) ---

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
            if (!response.IsSuccessStatusCode) return null;

            try
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    return loginResponse.Token;
                }
                return null;
            }
            catch
            {
                return null;
            }
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