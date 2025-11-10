using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class TipoExamenController : Controller
    {
        private readonly IApiClient _api;
        public TipoExamenController(IApiClient apiClient) => _api = apiClient;

        // GET: /TipoExamen/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var tipos = (await _api.GetTiposExamenAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                tipos = tipos.Where(t =>
                    t.Nombre.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase) ||
                    t.Descripcion.Contains(searchTerm, System.StringComparison.OrdinalIgnoreCase)).ToList();

            return View(tipos);
        }

        // GET: /TipoExamen/Create
        public IActionResult Create() => View(new TipoExamenDto { Estado = true });

        // POST: /TipoExamen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoExamenDto tipo)
        {
            if (!ModelState.IsValid)
                return View(tipo);

            var ok = await _api.CreateTipoExamenAsync(tipo);
            if (ok)
            {
                TempData["Success"] = "✅ Tipo de examen creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al crear el tipo de examen.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TipoExamen/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _api.GetTipoExamenAsync(id);
            if (tipo == null)
            {
                TempData["Error"] = "❌ Tipo de examen no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        // POST: /TipoExamen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoExamenDto tipo)
        {
            if (id != tipo.IdTipoExamen)
            {
                TempData["Error"] = "❌ Id inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(tipo);

            var ok = await _api.UpdateTipoExamenAsync(id, tipo);
            if (ok)
            {
                TempData["Success"] = "✅ Tipo de examen actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar el tipo de examen.";
            return View(tipo);
        }

        // GET: /TipoExamen/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _api.GetTipoExamenAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        // POST: /TipoExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _api.DeleteTipoExamenAsync(id);
            if (ok)
            {
                TempData["Success"] = "🗑️ Tipo de examen eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo eliminar el tipo de examen.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TipoExamen/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var tipo = await _api.GetTipoExamenAsync(id);
            if (tipo == null)
            {
                TempData["Error"] = "No se encontró el tipo de examen.";
                return RedirectToAction(nameof(Index));
            }

            tipo.Estado = !tipo.Estado;
            var ok = await _api.UpdateTipoExamenAsync(id, tipo);

            if (!ok)
                TempData["Error"] = "No se pudo cambiar el estado del tipo de examen.";

            return RedirectToAction(nameof(Index));
        }
    }
}

