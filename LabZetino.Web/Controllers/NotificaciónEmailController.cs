using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;

namespace LabZetino.Web.Controllers
{
    [Route("api/notificacionemail")]
    [ApiController]
    public class NotificacionEmailController : ControllerBase
    {
        private readonly NotificacionEmailService _notificacionService;

        public NotificacionEmailController(NotificacionEmailService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        // ✅ GET: api/notificacionemail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacionEmail>>> Get()
        {
            var notificaciones = await _notificacionService.ObtenerTodasLasNotificacionesAsync();
            return Ok(notificaciones);
        }

        // ✅ GET: api/notificacionemail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificacionEmail>> GetById(int id)
        {
            try
            {
                var notificacion = await _notificacionService.ObtenerNotificacionPorIdAsync(id);

                if (notificacion == null)
                    return NotFound($"No se encontró una notificación con ID {id}");

                return Ok(notificacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ GET: api/notificacionemail/resultado/10
        [HttpGet("resultado/{idResultado}")]
        public async Task<ActionResult<IEnumerable<NotificacionEmail>>> GetByResultado(int idResultado)
        {
            try
            {
                var notificaciones = await _notificacionService.ObtenerNotificacionesPorResultadoAsync(idResultado);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ GET: api/notificacionemail/estadoenvio/Enviado
        [HttpGet("estadoenvio/{estadoEnvio}")]
        public async Task<ActionResult<IEnumerable<NotificacionEmail>>> GetByEstadoEnvio(string estadoEnvio)
        {
            try
            {
                var notificaciones = await _notificacionService.ObtenerNotificacionesPorEstadoEnvioAsync(estadoEnvio);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ GET: api/notificacionemail/activas
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<NotificacionEmail>>> GetActivas()
        {
            try
            {
                var notificaciones = await _notificacionService.ObtenerNotificacionesActivasAsync();
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ POST: api/notificacionemail
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificacionEmail notificacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _notificacionService.AgregarNotificacionAsync(notificacion);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ✅ PUT: api/notificacionemail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] NotificacionEmail notificacion)
        {
            try
            {
                notificacion.IdNotificacion = id;

                var resultado = await _notificacionService.ModificarNotificacionAsync(notificacion);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ DELETE: api/notificacionemail/5  (borrado físico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _notificacionService.EliminarNotificacionAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ✅ PATCH: api/notificacionemail/cancelar/5 (soft delete)
        [HttpPatch("cancelar/{id}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var resultado = await _notificacionService.CancelarNotificacionAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
