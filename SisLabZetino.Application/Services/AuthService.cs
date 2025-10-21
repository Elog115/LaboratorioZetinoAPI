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

        // ===========================================================
        // 🔐 AUTENTICACIÓN (Register + Login + Token)
        // ===========================================================

        // Registrar usuario nuevo
        public async Task<(bool ok, string msg)> RegisterAsync(string nombre, string email, string password, int idRol)
        {
            var existing = await _repo.GetByEmailAsync(email);
            if (existing != null) return (false, "El email ya está registrado");

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
            return (true, "Usuario registrado");
        }

        // Login y generación de token
        public async Task<(bool ok, string tokenOrMsg)> LoginAsync(string correo, string password)
        {
            var user = await _repo.GetByEmailAsync(correo);
            if (user is null || !user.Estado) return (false, "Usuario no encontrado o inactivo");
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return (false, "Credenciales inválidas");

            var token = GenerateJwt(user);
            return (true, token);
        }

        // Generar JWT
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

        // ===========================================================
        // 👤 CRUD DE USUARIOS (Fusionado desde UsuarioService)
        // ===========================================================

        // Agregar usuario
        public async Task<string> AgregarUsuarioAsync(Usuario nuevoUsuario)
        {
            try
            {
                var usuarios = await _repo.GetUsuariosAsync();

                if (usuarios.Any(p => p.Nombre.ToLower() == nuevoUsuario.Nombre.ToLower()))
                    return "Error: Ya existe un usuario con el mismo nombre";

                nuevoUsuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.PasswordHash);
                nuevoUsuario.Estado = true;

                var usuarioInsertado = await _repo.AddUsuarioAsync(nuevoUsuario);

                if (usuarioInsertado == null || usuarioInsertado.IdUsuario <= 0)
                    return "Error: No se pudo agregar el Usuario";

                return "Usuario agregado correctamente";
            }
            catch (Exception ex)
            {
                return "Error de servidor: " + ex.Message;
            }
        }

        // Modificar usuario
        public async Task<string> ModificarUsuarioAsync(Usuario usuario)
        {
            if (usuario.IdUsuario <= 0)
                return "Error: ID no válido";

            var existente = await _repo.GetUsuarioByIdAsync(usuario.IdUsuario);

            if (existente == null)
                return "Error: Usuario no encontrado";

            existente.Nombre = usuario.Nombre;
            existente.Apellido = usuario.Apellido;
            existente.Email = usuario.Email;
            existente.FechaNacimiento = usuario.FechaNacimiento;
            existente.Telefono = usuario.Telefono;
            existente.IdRol = usuario.IdRol;
            existente.Estado = usuario.Estado;

            if (!string.IsNullOrWhiteSpace(usuario.PasswordHash))
                existente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);

            await _repo.UpdateUsuarioAsync(existente);
            return "Usuario modificado correctamente";
        }

        // Eliminar usuario (borrado lógico o real según el repo)
        public async Task<string> EliminarUsuarioAsync(int id)
        {
            var eliminado = await _repo.DeleteUsuarioAsync(id);
            if (!eliminado)
                return "Error: Usuario no encontrado";

            return "Usuario eliminado correctamente";
        }

        // Obtener usuario por ID (solo activos)
        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            if (id <= 0) return null;

            var usuario = await _repo.GetUsuarioByIdAsync(id);
            if (usuario != null && usuario.Estado)
                return usuario;

            return null;
        }

        // Obtener todos los usuarios activos
        public async Task<IEnumerable<Usuario>> ObtenerUsuariosActivosAsync()
        {
            var usuarios = await _repo.GetUsuariosAsync();
            return usuarios.Where(u => u.Estado == true);
        }

        // Validar usuario (sin generar token)
        public async Task<Usuario?> ValidateUsuarioAsync(string correo, string clave)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
                return null;

            var usuario = await _repo.ValidateUsuarioAsync(correo, clave);
            if (usuario != null && usuario.Estado)
                return usuario;

            return null;
        }
    }
}
