using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace LabZetino.Web.Controllers
{
    [Route("api/rol")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly RolService _rolService;

        public RolController(RolService rolService)
        {
            _rolService = rolService;
        }

        // GET: api/rol
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> Get()
        {
            var roles = await _rolService.ObtenerRolesActivosAsync();
            return Ok(roles);
        }

        // GET api/rol/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetById(int id)
        {
            try
            {
                var rol = await _rolService.ObtenerRolPorIdAsync(id);

                if (rol == null)
                    return NotFound($"No se encontró un rol activo con ID {id}");

                return Ok(rol);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/rol
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rol rol)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _rolService.AgregarRolAsync(rol);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT api/rol/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Rol rol)
        {
            try
            {
                rol.IdRol = id; // usamos el id de la ruta
                var resultado = await _rolService.ModificarRolAsync(rol);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE api/rol/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _rolService.EliminarRolAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
