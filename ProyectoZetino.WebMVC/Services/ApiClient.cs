
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoZetino.WebMVC.Models;
using System.Text.Json.Serialization;

namespace ProyectoZetino.WebMVC.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        private class LoginResponse
        {
            [JsonPropertyName("token")] // Asegura que coincida con el JSON ("token" en minúscula)
            public string? Token { get; set; }
        }

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // --- Roles (Estos ya estaban bien) ---
        public async Task<IEnumerable<RolDto>> GetRolesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RolDto>>("api/rol") ?? new List<RolDto>();
        }
        public async Task<RolDto> GetRolAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<RolDto>($"api/rol/{id}");
        }
        public async Task<bool> CreateRolAsync(RolDto rol)
        {
            var response = await _httpClient.PostAsJsonAsync("api/rol", rol);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateRolAsync(int id, RolDto rol)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/rol/{id}", rol);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteRolAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/rol/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            // El payload con datos de relleno (esto está bien)
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

            // ?? CORREGIDO: Usamos la clase 'LoginResponse' en lugar de 'dynamic'
            try
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    return loginResponse.Token; // ¡Éxito!
                }
                return null; // No vino el token
            }
            catch
            {
                return null; // Error al leer el JSON
            }
        }

        // --- Register (Este ya está corregido) ---
        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var payload = new
            {
                Nombre = model.Nombre,
                Email = model.Email,
                PasswordHash = model.Password,
                IdRol = 2, // 2 = Doctor

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