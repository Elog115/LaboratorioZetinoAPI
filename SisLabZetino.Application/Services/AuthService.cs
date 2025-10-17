using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories;

namespace SisLabZetino.Application.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IConfiguration _cfg;

        public AuthService(IUsuarioRepository repo, IConfiguration cfg)
        {
            _repo = repo;
            _cfg = cfg;
        }

        public async Task<(bool ok, string msg)> RegisterAsync(string nombre, string email, string password, int idRol)
        {
            var existing = await _repo.GetByEmailAsync(email);
            if (existing != null) return (false, "El correo ya está registrado");

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var usuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                PasswordHash = hash,
                IdRol = idRol,
                Estado = true
            };
            await _repo.AddUsuarioAsync(usuario);
            return (true, "Usuario registrado correctamente");
        }

        public async Task<(bool ok, string tokenOrMsg)> LoginAsync(string correo, string password)
        {
            var user = await _repo.GetByEmailAsync(correo);
            if (user is null || !user.Estado) return (false, "Usuario no encontrado o inactivo");
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return (false, "Credenciales inválidas");

            var token = GenerateJwt(user);
            return (true, token);
        }

        private string GenerateJwt(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Role, user.IdRol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
