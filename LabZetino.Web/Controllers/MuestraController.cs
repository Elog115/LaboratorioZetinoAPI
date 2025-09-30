using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace LabZetino.Web.Controllers
{
    [Route("api/muestra")]
    [ApiController]
    public class MuestraController : ControllerBase
    {
        private readonly MuestraService _muestraService;

        public MuestraController(MuestraService muestraService)
        {
            _muestraService = muestraService;
        }

        // GET: api/muestra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Muestra>>> Get()
        {
            var muestras = await _muestraService.ObtenerMuestrasActivasAsync();
            return Ok(muestras);
        }

        // GET api/muestra/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Muestra>> GetById(int id)
        {
            try
            {
                var muestra = await _muestraService.ObtenerMuestraPorIdAsync(id);

                if (muestra == null)
                    return NotFound($"No se encontró una muestra con ID {id}");

                return Ok(muestra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/muestra
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Muestra muestra)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _muestraService.AgregarMuestraAsync(muestra);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT api/muestra/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Muestra muestra)
        {
            try
            {
                muestra.IdMuestra = id; // usamos el id de la ruta
                var resultado = await _muestraService.ModificarMuestraAsync(muestra);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        //DELETE api/examen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _muestraService.EliminarMuestraAsync(id);
            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);
            return Ok(resultado);
        }


    }
}
