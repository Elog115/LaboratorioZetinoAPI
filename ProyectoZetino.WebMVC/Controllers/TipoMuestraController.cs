using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;    
using ProyectoZetino.WebMVC.Services; 
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class TipoMuestraController : Controller
    {
        private readonly IApiClient _api;

        public TipoMuestraController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /TipoMuestra/
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var tiposMuestra = await _api.GetTiposMuestraAsync(searchTerm);
            return View(tiposMuestra);
        }

        // GET: /TipoMuestra/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /TipoMuestra/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoMuestraDto tipoMuestra)
        {
            if (!ModelState.IsValid)
                return View(tipoMuestra);

            // Replicamos la regla de negocio de RolController:
            // Todos los nuevos se crean como Activos (true)
            tipoMuestra.Estado = true;

            var success = await _api.CreateTipoMuestraAsync(tipoMuestra);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el Tipo de Muestra.");
            return View(tipoMuestra);
        }

        // GET: /TipoMuestra/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipoMuestra = await _api.GetTipoMuestraAsync(id);
            if (tipoMuestra == null)
                return NotFound();

            return View(tipoMuestra);
        }

        // POST: /TipoMuestra/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoMuestraDto tipoMuestra)
        {
            if (id != tipoMuestra.IdTipoMuestra)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(tipoMuestra);

            var success = await _api.UpdateTipoMuestraAsync(id, tipoMuestra);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el Tipo de Muestra.");
            return View(tipoMuestra);
        }

        // GET: /TipoMuestra/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var tipoMuestra = await _api.GetTipoMuestraAsync(id);
            if (tipoMuestra == null)
            {
                TempData["Error"] = "No se encontró el tipo de muestra.";
                return RedirectToAction(nameof(Index));
            }

            // Invertimos el estado
            tipoMuestra.Estado = !tipoMuestra.Estado;

            var success = await _api.UpdateTipoMuestraAsync(id, tipoMuestra);

            if (!success)
            {
                TempData["Error"] = "Error al cambiar el estado del tipo de muestra.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /TipoMuestra/Lista  -> devuelve solo la tabla para AJAX
        [HttpGet]
        public async Task<IActionResult> Lista(string q = null)
        {
            var tiposMuestra = await _api.GetTiposMuestraAsync(q);

            // Asumiendo que crearás esta vista parcial
            return PartialView("_TablaTiposMuestra", tiposMuestra);
        }
    }
}