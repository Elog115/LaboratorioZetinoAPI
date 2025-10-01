using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories; // Asegúrate de tener la interfaz aquí
using System;
using System.Threading.Tasks;

namespace SisLabZetino.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionEmailController : ControllerBase
    {
        private readonly INotificacionEmailRepository _repository;

        public NotificacionEmailController(INotificacionEmailRepository repository)
        {
            _repository = repository;
        }

        // ✅ GET: api/NotificacionEmail
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notificaciones = await _repository.GetNotificacionesAsync();
            return Ok(notificaciones);
        }

        // ✅ GET: api/NotificacionEmail/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notificacion = await _repository.GetNotificacionByIdAsync(id);
            if (notificacion == null)
                return NotFound(new { message = $"No se encontró la notificación con Id {id}" });

            return Ok(notificacion);
        }

        // ✅ GET: api/NotificacionEmail/resultado/10
        [HttpGet("resultado/{idResultado:int}")]
        public async Task<IActionResult> GetByResultado(int idResultado)
        {
            var notificaciones = await _repository.GetNotificacionesByResultadoAsync(idResultado);
            return Ok(notificaciones);
        }

        // ✅ GET: api/NotificacionEmail/estadoenvio/Enviado
        [HttpGet("estadoenvio/{estadoEnvio}")]
        public async Task<IActionResult> GetByEstadoEnvio(string estadoEnvio)
        {
            var notificaciones = await _repository.GetNotificacionesByEstadoEnvioAsync(estadoEnvio);
            return Ok(notificaciones);
        }

        // ✅ GET: api/NotificacionEmail/estado/1
        [HttpGet("estado/{estado:int}")]
        public async Task<IActionResult> GetByEstado(bool estado)
        {
            var notificaciones = await _repository.GetNotificacionesByEstadoAsync(estado);
            return Ok(notificaciones);
        }

        // ✅ POST: api/NotificacionEmail
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificacionEmail notificacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nuevaNotificacion = await _repository.AddNotificacionAsync(notificacion);
            return CreatedAtAction(nameof(GetById), new { id = nuevaNotificacion.IdNotificacion }, nuevaNotificacion);
        }

        // ✅ PUT: api/NotificacionEmail/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] NotificacionEmail notificacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != notificacion.IdNotificacion)
                return BadRequest(new { message = "El Id del cuerpo no coincide con el de la URL." });

            var existente = await _repository.GetNotificacionByIdAsync(id);
            if (existente == null)
                return NotFound(new { message = $"No se encontró la notificación con Id {id}" });

            await _repository.UpdateNotificacionAsync(notificacion);
            return NoContent();
        }

        // ✅ DELETE: api/NotificacionEmail/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _repository.DeleteNotificacionAsync(id);
            if (!eliminado)
                return NotFound(new { message = $"No se encontró la notificación con Id {id}" });

            return NoContent();
        }
    }
}
