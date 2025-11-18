using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabZetino.Web.Controllers
{
    [Route("api/orden-examen")]
    [ApiController]
    public class OrdenExamenController : ControllerBase
    {
        private readonly OrdenExamenService _ordenExamenService;

        public OrdenExamenController(OrdenExamenService ordenExamenService)
        {
            _ordenExamenService = ordenExamenService;
        }

        // ✅ GET: api/orden-examen
        // Este método ahora devuelve SOLO las órdenes activas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenExamen>>> GetTodas()
        {
            // CAMBIO REALIZADO AQUÍ:
            // Usamos ObtenerOrdenesActivasAsync en lugar de ObtenerTodasLasOrdenesAsync
            var ordenes = await _ordenExamenService.ObtenerOrdenesActivasAsync();
            return Ok(ordenes);
        }

        // ✅ GET: api/orden-examen/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenExamen>> GetPorId(int id)
        {
            try
            {
                var orden = await _ordenExamenService.ObtenerOrdenPorIdAsync(id);
                if (orden == null)
                    return NotFound($"No se encontró una orden con ID {id}");

                return Ok(orden);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ GET: api/orden-examen/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<OrdenExamen>>> GetPorUsuario(int idUsuario)
        {
            var ordenes = await _ordenExamenService.ObtenerOrdenesPorUsuarioAsync(idUsuario);
            return Ok(ordenes);
        }

        // ✅ GET: api/orden-examen/cita/{idCita}
        [HttpGet("cita/{idCita}")]
        public async Task<ActionResult<IEnumerable<OrdenExamen>>> GetPorCita(int idCita)
        {
            var ordenes = await _ordenExamenService.ObtenerOrdenesPorCitaAsync(idCita);
            return Ok(ordenes);
        }

        // ✅ GET: api/orden-examen/activas
        // (Este endpoint sigue existiendo, aunque GetTodas ahora hace lo mismo)
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<OrdenExamen>>> GetActivas()
        {
            var ordenes = await _ordenExamenService.ObtenerOrdenesActivasAsync();
            return Ok(ordenes);
        }

        // ✅ GET: api/orden-examen/fecha/{fechaSolicitud}
        [HttpGet("fecha/{fechaSolicitud}")]
        public async Task<ActionResult<IEnumerable<OrdenExamen>>> GetPorFecha(DateTime fechaSolicitud)
        {
            var ordenes = await _ordenExamenService.ObtenerOrdenesPorFechaSolicitudAsync(fechaSolicitud);
            return Ok(ordenes);
        }

        // ✅ POST: api/orden-examen
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrdenExamen orden)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _ordenExamenService.AgregarOrdenAsync(orden);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ✅ PUT: api/orden-examen/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrdenExamen orden)
        {
            try
            {
                orden.IdOrdenExamen = id; // Usar el id de la ruta
                var resultado = await _ordenExamenService.ModificarOrdenAsync(orden);

                if (resultado.StartsWith("Error"))
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ✅ DELETE: api/orden-examen/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _ordenExamenService.EliminarOrdenAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // ✅ PATCH: api/orden-examen/cancelar/{id}
        [HttpPatch("cancelar/{id}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var resultado = await _ordenExamenService.CancelarOrdenAsync(id);

            if (resultado.StartsWith("Error"))
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}