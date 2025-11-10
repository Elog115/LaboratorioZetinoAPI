using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class ExamenController : Controller
    {
        private readonly IApiClient _api;
        public ExamenController(IApiClient apiClient) => _api = apiClient;

        // GET: /Examen/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var examenes = (await _api.GetExamenesAsync(searchTerm)).ToList();
            var ordenes = await _api.GetOrdenesExamenAsync();
            var tipos = await _api.GetTiposExamenAsync();

            var mapaOrdenes = ordenes.ToDictionary(o => o.IdOrdenExamen, o => $"Orden #{o.IdOrdenExamen}");
            var mapaTipos = tipos.ToDictionary(t => t.IdTipoExamen, t => t.Nombre);

            foreach (var e in examenes)
            {
                e.NombreOrden = mapaOrdenes.ContainsKey(e.IdOrdenExamen)
                    ? mapaOrdenes[e.IdOrdenExamen]
                    : "Desconocida";

                e.NombreTipoExamen = mapaTipos.ContainsKey(e.IdTipoExamen)
                    ? mapaTipos[e.IdTipoExamen]
                    : "Desconocido";
            }

            return View(examenes);
        }

        // GET: /Examen/Create
        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View(new ExamenDto { Estado = true });
        }

        // POST: /Examen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamenDto examen)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas(examen.IdOrdenExamen, examen.IdTipoExamen);
                return View(examen);
            }

            var ok = await _api.CreateExamenAsync(examen);
            if (ok)
            {
                TempData["Success"] = "✅ Examen creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al crear el examen.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Examen/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var examen = await _api.GetExamenAsync(id);
            if (examen == null)
            {
                TempData["Error"] = "❌ Examen no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListas(examen.IdOrdenExamen, examen.IdTipoExamen);
            return View(examen);
        }

        // POST: /Examen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamenDto examen)
        {
            if (id != examen.IdExamen)
            {
                TempData["Error"] = "❌ Id inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CargarListas(examen.IdOrdenExamen, examen.IdTipoExamen);
                return View(examen);
            }

            var ok = await _api.UpdateExamenAsync(id, examen);
            if (ok)
            {
                TempData["Success"] = "✅ Examen actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar el examen.";
            await CargarListas(examen.IdOrdenExamen, examen.IdTipoExamen);
            return View(examen);
        }

        // GET: /Examen/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var examen = await _api.GetExamenAsync(id);
            if (examen == null) return NotFound();
            return View(examen);
        }

        // POST: /Examen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _api.DeleteExamenAsync(id);
            if (ok)
            {
                TempData["Success"] = "🗑️ Examen eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo eliminar el examen.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Examen/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var examen = await _api.GetExamenAsync(id);
            if (examen == null)
            {
                TempData["Error"] = "No se encontró el examen.";
                return RedirectToAction(nameof(Index));
            }

            examen.Estado = !examen.Estado;

            var ok = await _api.UpdateExamenAsync(id, examen);
            if (!ok)
                TempData["Error"] = "No se pudo cambiar el estado del examen.";

            return RedirectToAction(nameof(Index));
        }

        // ✅ Cargar listas corregido: usando propiedades reales
        private async Task CargarListas(int? ordenSeleccionada = null, int? tipoSeleccionado = null)
        {
            var ordenes = await _api.GetOrdenesExamenAsync();
            var tipos = await _api.GetTiposExamenAsync();

            var ordenesFormateadas = ordenes.Select(o => new
            {
                o.IdOrdenExamen,
                Nombre = $"Orden #{o.IdOrdenExamen}"
            });

            ViewBag.Ordenes = new SelectList(ordenesFormateadas, "IdOrdenExamen", "Nombre", ordenSeleccionada);

            ViewBag.TiposExamen = new SelectList(tipos, "IdTipoExamen", "Nombre", tipoSeleccionado);
        }
    }
}
