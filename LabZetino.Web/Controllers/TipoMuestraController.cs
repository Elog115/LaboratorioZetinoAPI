using Microsoft.AspNetCore.Mvc;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Domain.Repositories; // <-- Tu interfaz
using System.Threading.Tasks;

namespace SisLabZetino.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tipomuestra")] // <-- Ruta en minúsculas
    public class TipoMuestraController : ControllerBase
    {
        private readonly ITipoMuestraRepository _repository;

        public TipoMuestraController(ITipoMuestraRepository repository)
        {
            _repository = repository;
        }

        // --- MÉTODOS CRUD ADAPTADOS A TU INTERFAZ ---

        // GET: api/tipomuestra
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Tu interfaz no tiene filtro, así que llamamos al método sin parámetro
            var tiposMuestra = await _repository.GetTiposMuestraAsync();
            return Ok(tiposMuestra);
        }

        // GET: api/tipomuestra/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tipoMuestra = await _repository.GetTipoMuestraByIdAsync(id);
            if (tipoMuestra == null)
                return NotFound(new { message = $"No se encontró el tipo de muestra con Id {id}" });

            return Ok(tipoMuestra);
        }

        // POST: api/tipomuestra
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoMuestra tipoMuestra)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nuevoTipoMuestra = await _repository.AddTipoMuestraAsync(tipoMuestra);

            return CreatedAtAction(nameof(GetById), new { id = nuevoTipoMuestra.IdTipoMuestra }, nuevoTipoMuestra);
        }

        // PUT: api/tipomuestra/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoMuestra tipoMuestra)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != tipoMuestra.IdTipoMuestra)
                return BadRequest(new { message = "El Id del cuerpo no coincide con el de la URL." });

            var existente = await _repository.GetTipoMuestraByIdAsync(id);
            if (existente == null)
                return NotFound(new { message = $"No se encontró el tipo de muestra con Id {id}" });

            // Tu repositorio devuelve el objeto actualizado, así que lo capturamos
            var tipoMuestraActualizado = await _repository.UpdateTipoMuestraAsync(tipoMuestra);

            // Devolvemos 200 OK con el objeto
            return Ok(tipoMuestraActualizado);
        }

        // DELETE: api/tipomuestra/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existente = await _repository.GetTipoMuestraByIdAsync(id);
            if (existente == null)
                return NotFound(new { message = $"No se encontró el tipo de muestra con Id {id}" });

            var eliminado = await _repository.DeleteTipoMuestraAsync(id);
            if (!eliminado)
                return BadRequest(new { message = "No se pudo eliminar el tipo de muestra." });

            return NoContent(); // 204 Éxito, sin contenido
        }
    }
}