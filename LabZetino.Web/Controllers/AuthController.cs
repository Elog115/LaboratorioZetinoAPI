using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest(new { message = "Datos de usuario requeridos" });

            var (ok, msg) = await _authService.RegisterAsync(usuario.Nombre, usuario.Email, usuario.PasswordHash!, usuario.IdRol);
            if (!ok) return BadRequest(new { message = msg });

            return Ok(new { message = msg });
        }

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
    }
}
