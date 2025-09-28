using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace SisLabZetino.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadoController : ControllerBase
    {
        private readonly ResultadoService _resultadoService;

        public ResultadoController(ResultadoService resultadoService)
        {
            _resultadoService = resultadoService;
        }

        // ✅ GET api/resultado
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resultados = await _resultadoService.ObtenerTodosLosResultadosAsync();
            return Ok(resultados);
        }

        // ✅ GET api/resultado/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _resultadoService.ObtenerResultadoPorIdAsync(id);
            if (resultado == null)
                return NotFound(new { message = "Resultado no encontrado" });

            return Ok(resultado);
        }

        // ✅ GET api/resultado/examen/5
        [HttpGet("examen/{idExamen:int}")]
        public async Task<IActionResult> GetByExamen(int idExamen)
        {
            var resultados = await _resultadoService.ObtenerResultadosPorExamenAsync(idExamen);
            return Ok(resultados);
        }

        // ✅ GET api/resultado/fecha/2025-09-26
        [HttpGet("fecha/{fechaEntrega}")]
        public async Task<IActionResult> GetByFecha(DateTime fechaEntrega)
        {
            var resultados = await _resultadoService.ObtenerResultadosPorFechaEntregaAsync(fechaEntrega);
            return Ok(resultados);
        }

        // ✅ GET api/resultado/activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            var resultados = await _resultadoService.ObtenerResultadosActivosAsync();
            return Ok(resultados);
        }

        // ✅ POST api/resultado
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Resultado nuevoResultado)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = await _resultadoService.AgregarResultadoAsync(nuevoResultado);
            return Ok(new { message = mensaje });
        }

        // ✅ PUT api/resultado/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Resultado resultado)
        {
            if (id != resultado.IdResultado)
                return BadRequest(new { message = "El ID del body no coincide con el de la URL" });

            var mensaje = await _resultadoService.ModificarResultadoAsync(resultado);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // ✅ DELETE api/resultado/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _resultadoService.EliminarResultadoAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        // ✅ PATCH api/resultado/cancelar/{id}
        [HttpPatch("cancelar/{id:int}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var mensaje = await _resultadoService.CancelarResultadoAsync(id);
            if (mensaje.StartsWith("Error"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }
    }
}
