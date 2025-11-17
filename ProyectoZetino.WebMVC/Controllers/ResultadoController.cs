using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // <-- Necesario para SelectList
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq; // <-- Necesario para .Where() y .OrderBy()
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class ResultadoController : Controller
    {
        private readonly IApiClient _api;

        public ResultadoController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /Resultado/
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var resultados = await _api.GetResultadosAsync(searchTerm);

            // --- 👇 LÍNEAS AGREGADAS (ESTA ES LA CORRECCIÓN) 👇 ---
            // Cargar exámenes para que la vista pueda mostrar la descripción
            var examenes = await _api.GetExamenesAsync();
            ViewBag.ListaExamenes = examenes ?? new List<ExamenDto>();
            // --- 👆 FIN DE LA CORRECCIÓN 👆 ---

            return View(resultados);
        }

        // GET: /Resultado/Create
        public async Task<IActionResult> Create()
        {
            // Cargamos los exámenes activos para el dropdown
            await CargarExamenesDropdown();
            return View();
        }

        // POST: /Resultado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultadoDto resultado)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo falla, recargamos el dropdown y volvemos a la vista
                await CargarExamenesDropdown(resultado.IdExamen);
                return View(resultado);
            }

            resultado.Estado = true;

            var success = await _api.CreateResultadoAsync(resultado);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el resultado.");
            // Recargamos el dropdown si la creación en API falla
            await CargarExamenesDropdown(resultado.IdExamen);
            return View(resultado);
        }

        // GET: /Resultado/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
            if (resultado == null)
                return NotFound();

            // Cargamos los exámenes y pre-seleccionamos el actual
            await CargarExamenesDropdown(resultado.IdExamen);
            return View(resultado);
        }

        // POST: /Resultado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResultadoDto resultado)
        {
            if (id != resultado.IdResultado)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                // Si el modelo falla, recargamos el dropdown
                await CargarExamenesDropdown(resultado.IdExamen);
                return View(resultado);
            }

            var success = await _api.UpdateResultadoAsync(id, resultado);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el resultado.");
            await CargarExamenesDropdown(resultado.IdExamen);
            return View(resultado);
        }

        // GET: /Resultado/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado.";
                return RedirectToAction(nameof(Index));
            }

            resultado.Estado = !resultado.Estado;
            var success = await _api.UpdateResultadoAsync(id, resultado);

            if (!success)
            {
                TempData["Error"] = "Error al cambiar el estado del resultado.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Resultado/Lista  -> devuelve solo la tabla para AJAX
        [HttpGet]
        public async Task<IActionResult> Lista(string q = null)
        {
            var resultados = await _api.GetResultadosAsync(q);
            // La vista parcial también necesita la lista de exámenes
            var examenes = await _api.GetExamenesAsync();
            ViewBag.ListaExamenes = examenes ?? new List<ExamenDto>();

            return PartialView("_TablaResultados", resultados);
        }


        // --- MÉTODO HELPER PRIVADO ---
        private async Task CargarExamenesDropdown(object? selectedValue = null)
        {
            var examenes = await _api.GetExamenesAsync();

            ViewBag.Examenes = new SelectList(
                examenes.Where(e => e.Estado)
                        .OrderBy(e => e.Descripcion),
                "IdExamen",
                "Descripcion",
                selectedValue
            );
        }
    }
}