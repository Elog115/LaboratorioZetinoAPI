using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace SisLabZetino.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioSistemaController : ControllerBase
    {
        private readonly UsuarioSistemaService _usuarioService;

        public UsuarioSistemaController(UsuarioSistemaService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET api/usuariosistema
        [HttpGet]
        public async Task<IActionResult> GetAllActivos()
        {
            var usuarios = await _usuarioService.ObtenerUsuariosActivosAsync();
            return Ok(usuarios);
        }

        // GET api/usuariosistema/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado o inactivo" });

            return Ok(usuario);
        }

        // POST api/usuariosistema
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioSistema nuevoUsuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _usuarioService.AgregarUsuarioAsync(nuevoUsuario);
            if (mensaje.StartsWith("Error"))
                return BadRequest(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // PUT api/usuariosistema/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioSistema usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest(new { message = "El ID del body no coincide con el de la URL" });

            var mensaje = await _usuarioService.ModificarUsuarioAsync(usuario);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // DELETE api/usuariosistema/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _usuarioService.EliminarUsuarioAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // POST api/usuariosistema/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Correo) || string.IsNullOrEmpty(login.Clave))
                return BadRequest(new { message = "Correo y clave son requeridos" });

            var usuario = await _usuarioService.ValidarUsuarioAsync(login.Correo, login.Clave);

            if (usuario == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            return Ok(usuario);
        }
    }

    // DTO para login
    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Clave { get; set; }
    }
}
