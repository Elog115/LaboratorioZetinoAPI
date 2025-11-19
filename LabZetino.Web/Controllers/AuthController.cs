using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using System.Threading.Tasks;

namespace SisLabZetino.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // ✅ Registro
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest(new { message = "Datos de usuario requeridos" });

            var (ok, msg) = await _authService.RegisterAsync(
                usuario.Nombre,
                usuario.Email,
                usuario.PasswordHash!,
                usuario.IdRol
            );

            if (!ok) return BadRequest(new { message = msg });
            return Ok(new { message = msg });
        }

        // ✅ Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.PasswordHash))
                return BadRequest(new { message = "Correo y clave son requeridos" });

            var (ok, tokenOrMsg) = await _authService.LoginAsync(usuario.Email, usuario.PasswordHash!);
            if (!ok) return Unauthorized(new { message = tokenOrMsg });

            return Ok(new { token = tokenOrMsg });
        }

        // ✅ Obtener todos los usuarios activos
        [HttpGet("usuarios")]
        
        public async Task<IActionResult> GetAllActivos()
        {
            var usuarios = await _authService.ObtenerUsuariosActivosAsync();
            return Ok(usuarios);
        }

        // ✅ Obtener usuario por ID
        [HttpGet("usuarios/{id:int}")]
        
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _authService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado o inactivo" });

            return Ok(usuario);
        }

        // ✅ Crear nuevo usuario
        [HttpPost("usuarios")]
        
        public async Task<IActionResult> Create([FromBody] Usuario nuevoUsuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _authService.AgregarUsuarioAsync(nuevoUsuario);
            if (mensaje.StartsWith("Error"))
                return BadRequest(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // ✅ Modificar usuario
        [HttpPut("usuarios/{id:int}")]
        
        public async Task<IActionResult> Update(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest(new { message = "El ID del body no coincide con el de la URL" });

            var mensaje = await _authService.ModificarUsuarioAsync(usuario);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // ✅ Eliminar usuario
        [HttpDelete("usuarios/{id:int}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _authService.EliminarUsuarioAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }
        [HttpPatch("usuarios/{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var (ok, nuevoEstado, mensaje) = await _authService.CambiarEstadoUsuarioAsync(id);

            if (!ok)
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje, estado = nuevoEstado });
        }


    }
}
