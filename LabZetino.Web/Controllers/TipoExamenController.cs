using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace SisLabZetino.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tipo-examen")]
    public class TipoExamenController : ControllerBase
    {
        private readonly TipoExamenService _tipoExamenService;

        public TipoExamenController(TipoExamenService tipoExamenService)
        {
            _tipoExamenService = tipoExamenService;
        }

        // GET api/tipoexamen
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _tipoExamenService.ObtenerTodosLosTiposExamenAsync();
            return Ok(tipos);
        }

        // GET api/tipoexamen/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _tipoExamenService.ObtenerTipoExamenPorIdAsync(id);
            if (tipo == null)
                return NotFound(new { message = "Tipo de examen no encontrado" });

            return Ok(tipo);
        }

        // GET api/tipoexamen/nombre/{nombre}
        [HttpGet("nombre/{nombre}")]
        public async Task<IActionResult> GetByNombre(string nombre)
        {
            var tipo = await _tipoExamenService.ObtenerTipoExamenPorNombreAsync(nombre);
            if (tipo == null)
                return NotFound(new { message = "Tipo de examen no encontrado" });

            return Ok(tipo);
        }

        // GET api/tipoexamen/activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            var tipos = await _tipoExamenService.ObtenerTiposExamenActivosAsync();
            return Ok(tipos);
        }

        // GET api/tipoexamen/precio?min=100&max=500
        [HttpGet("precio")]
        public async Task<IActionResult> GetByPrecio([FromQuery] decimal min, [FromQuery] decimal max)
        {
            var tipos = await _tipoExamenService.ObtenerTiposExamenPorPrecioAsync(min, max);
            return Ok(tipos);
        }

        // POST api/tipoexamen
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoExamen nuevoTipo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _tipoExamenService.AgregarTipoExamenAsync(nuevoTipo);
            return Ok(new { message = mensaje });
        }

        // PUT api/tipoexamen/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoExamen tipo)
        {
            if (id != tipo.IdTipoExamen)
                return BadRequest(new { message = "El ID del body no coincide con el de la URL" });

            var mensaje = await _tipoExamenService.ModificarTipoExamenAsync(tipo);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // DELETE api/tipoexamen/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _tipoExamenService.EliminarTipoExamenAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // PATCH api/tipoexamen/cancelar/{id}
        [HttpPatch("cancelar/{id:int}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var mensaje = await _tipoExamenService.CancelarTipoExamenAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }
    }
}
