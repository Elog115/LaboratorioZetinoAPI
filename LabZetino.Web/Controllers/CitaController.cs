using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace LabZetino.Web.Controllers
{
    [Route("api/cita")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly CitaService _citaService;

        public CitaController(CitaService citaService)
        {
            _citaService = citaService;
        }

        // GET: api/cita/Get
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> Get()
        {
            var citas = await _citaService.ObtenerCitasActivasAsync();
            return Ok(citas);
        }

        // GET api/citacontroller/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetById(int id)
        {
            try
            {
                var cita = await _citaService.ObtenerCitaPorIdAsync(id);

                if (cita == null)
                    return NotFound($"No se encontró una cita con ID {id}");

                return Ok(cita);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/citacontroller
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cita cita)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _citaService.AgregarCitaAsync(cita);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT api/citacontroller/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Cita cita)
        {
            try
            {
                // El servicio valida si el id es válido o no coincide, no lo hacemos aquí
                cita.IdCita = id; // nos aseguramos de que use el id de la ruta

                var resultado = await _citaService.ModificarCitaAsync(cita);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // 🔍 Registrar log aquí si tienes ILogger
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE api/citacontroller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _citaService.EliminarCitaAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
