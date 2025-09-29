using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace LabZetino.Web.Controllers
{
    [Route("api/examen")]
    [ApiController]
    public class ExamenController : ControllerBase
    {
        private readonly ExamenService _examenService;

        public ExamenController(ExamenService examenService)
        {
            _examenService = examenService;
        }

        // GET: api/examen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Examen>>> Get()
        {
            var examenes = await _examenService.ObtenerExamenesActivosAsync();
            return Ok(examenes);
        }

        // GET api/examen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Examen>> GetById(int id)
        {
            try
            {
                var examen = await _examenService.ObtenerExamenPorIdAsync(id);

                if (examen == null)
                    return NotFound($"No se encontró un examen con ID {id}");

                return Ok(examen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/examen
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Examen examen)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _examenService.AgregarExamenAsync(examen);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT api/examen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Examen examen)
        {
            try
            {
                examen.IdExamen = id; // Aseguramos que use el id de la ruta
                var resultado = await _examenService.ModificarExamenAsync(examen);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

       
    }
}
