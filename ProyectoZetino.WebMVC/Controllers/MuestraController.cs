using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Para los SelectList
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class MuestraController : Controller
    {
        private readonly IApiClient _api;

        public MuestraController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // --- MÉTODO PRIVADO ---
        // Para cargar los <select> de Orden de Examen y Tipo de Muestra
        private async Task PopulateDropdowns()
        {
            // OJO: Asumo que tienes estos métodos en tu ApiClient
            var ordenes = await _api.GetOrdenesExamenAsync(null);
            var tipos = await _api.GetTiposMuestraAsync(null);

            ViewBag.OrdenesExamen = new SelectList(ordenes, "IdOrdenExamen", "IdOrdenExamen"); // Ajusta "NombrePaciente" si lo tienes
            ViewBag.TiposMuestra = new SelectList(tipos, "IdTipoMuestra", "Nombre");
        }


        // GET: /Muestra/
        public async Task<IActionResult> Index(string searchTerm)
        {
            // El filtro JS client-side usará esto
            ViewData["CurrentFilter"] = searchTerm;

            // La API no usa 'searchTerm', así que llamamos sin él
            var muestras = await _api.GetMuestrasAsync();
            return View(muestras);
        }

        // GET: /Muestra/Create
        public async Task<IActionResult> Create()
        {
            // Llenamos los dropdowns antes de mostrar la vista
            await PopulateDropdowns();
            return View();
        }

        // POST: /Muestra/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MuestraDto muestra)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(); // Recargamos dropdowns si falla
                return View(muestra);
            }

            muestra.Estado = true; // Estado activo por defecto

            var resultado = await _api.CreateMuestraAsync(muestra);

            // Verificamos el string de respuesta de la API
            if (resultado != null && !resultado.StartsWith("Error"))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", resultado ?? "Error desconocido al crear la muestra.");
            await PopulateDropdowns(); // Recargamos dropdowns si falla
            return View(muestra);
        }

        // GET: /Muestra/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var muestra = await _api.GetMuestraAsync(id);
            if (muestra == null)
                return NotFound();

            // Llenamos los dropdowns
            await PopulateDropdowns();
            return View(muestra);
        }

        // POST: /Muestra/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MuestraDto muestra)
        {
            if (id != muestra.IdMuestra)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(); // Recargamos dropdowns si falla
                return View(muestra);
            }

            var resultado = await _api.UpdateMuestraAsync(id, muestra);

            // Verificamos el string de respuesta de la API
            if (resultado != null && !resultado.StartsWith("Error"))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", resultado ?? "Error desconocido al actualizar.");
            await PopulateDropdowns(); // Recargamos dropdowns si falla
            return View(muestra);
        }

        // GET: /Muestra/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var muestra = await _api.GetMuestraAsync(id);
            if (muestra == null)
            {
                TempData["Error"] = "No se encontró la muestra.";
                return RedirectToAction(nameof(Index));
            }

            // Invertimos el estado
            muestra.Estado = !muestra.Estado;

            var resultado = await _api.UpdateMuestraAsync(id, muestra);

            if (resultado.StartsWith("Error"))
            {
                TempData["Error"] = resultado;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}