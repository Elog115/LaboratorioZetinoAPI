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

        // ?? --- CÓDIGO RESTAURADO --- ??
        public async Task<IEnumerable<RolDto>> GetRolesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RolDto>>("api/rol") ?? new List<RolDto>();
        }

        // ?? --- CÓDIGO RESTAURADO --- ??
        public async Task<RolDto> GetRolAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<RolDto>($"api/rol/{id}");
        }

        // ?? --- CÓDIGO CORREGIDO (CON DESCRIPCION) --- ??
        public async Task<bool> CreateRolAsync(RolDto rol)
        {
            // Ahora enviamos el RolDto completo, ya que la API lo acepta
            var payload = new
            {
                rol.Nombre,
                rol.Estado,
                rol.Descripcion // <--- ¡LÍNEA AÑADIDA!
            };
            var response = await _httpClient.PostAsJsonAsync("api/rol", payload);
            return response.IsSuccessStatusCode;
        }

        // ?? --- CÓDIGO CORREGIDO (CON DESCRIPCION) --- ??
        public async Task<bool> UpdateRolAsync(int id, RolDto rol)
        {
            // Ahora enviamos el RolDto completo
            var payload = new
            {
                rol.IdRol,
                rol.Nombre,
                rol.Estado,
                rol.Descripcion // <--- ¡LÍNEA AÑADIDA!
            };
            var response = await _httpClient.PutAsJsonAsync($"api/rol/{id}", payload);
            return response.IsSuccessStatusCode;
        }

        // ?? --- CÓDIGO RESTAURADO --- ??
        public async Task<bool> DeleteRolAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/rol/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Login y Register (Restaurados) ---

        // ?? --- CÓDIGO RESTAURADO --- ??
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

        // ?? --- CÓDIGO RESTAURADO --- ??
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