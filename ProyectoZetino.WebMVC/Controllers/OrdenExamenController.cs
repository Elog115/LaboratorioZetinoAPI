using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class OrdenExamenController : Controller
    {
        private readonly IApiClient _api;
        public OrdenExamenController(IApiClient apiClient) => _api = apiClient;

        // GET: /OrdenExamen/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var ordenes = (await _api.GetOrdenesExamenAsync()).ToList();

            var usuarios = await _api.GetUsuariosAsync();


            var citas = await _api.GetCitasAsync(searchTerm);

            var mapaUsuarios = usuarios.ToDictionary(u => u.IdUsuario, u => $"{u.Nombre} {u.Apellido}");
            var mapaCitas = citas.ToDictionary(c => c.IdCita, c => c.Descripcion);

            foreach (var o in ordenes)
            {
                o.NombreUsuario = mapaUsuarios.ContainsKey(o.IdUsuario) ? mapaUsuarios[o.IdUsuario] : "Desconocido";
                o.DetalleCita = mapaCitas.ContainsKey(o.IdCita) ? mapaCitas[o.IdCita] : "Sin descripción";
            }

            return View(ordenes);
        }

        // GET: /OrdenExamen/Create
        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new OrdenExamenDto { Estado = true, FechaSolicitud = System.DateTime.Now });
        }

        // POST: /OrdenExamen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdenExamenDto orden)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(orden.IdUsuario, orden.IdCita);
                return View(orden);
            }

            var ok = await _api.CreateOrdenExamenAsync(orden);
            if (ok)
            {
                TempData["Success"] = "✅ Orden creada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al crear la orden.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /OrdenExamen/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var orden = await _api.GetOrdenExamenAsync(id);
            if (orden == null)
            {
                TempData["Error"] = "❌ Orden no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            await CargarCombos(orden.IdUsuario, orden.IdCita);
            return View(orden);
        }

        // POST: /OrdenExamen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrdenExamenDto orden)
        {
            if (id != orden.IdOrdenExamen)
            {
                TempData["Error"] = "❌ Id inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(orden.IdUsuario, orden.IdCita);
                return View(orden);
            }

            var ok = await _api.UpdateOrdenExamenAsync(id, orden);
            if (ok)
            {
                TempData["Success"] = "✅ Orden actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar la orden.";
            await CargarCombos(orden.IdUsuario, orden.IdCita);
            return View(orden);
        }

        // GET: /OrdenExamen/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var orden = await _api.GetOrdenExamenAsync(id);
            if (orden == null) return NotFound();
            return View(orden);
        }

        // POST: /OrdenExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _api.DeleteOrdenExamenAsync(id);
            if (ok)
            {
                TempData["Success"] = "🗑️ Orden eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo eliminar la orden.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /OrdenExamen/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var orden = await _api.GetOrdenExamenAsync(id);
            if (orden == null)
            {
                TempData["Error"] = "No se encontró la orden.";
                return RedirectToAction(nameof(Index));
            }

            orden.Estado = !orden.Estado;

            var ok = await _api.UpdateOrdenExamenAsync(id, orden);
            if (!ok)
                TempData["Error"] = "No se pudo cambiar el estado de la orden.";

            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private async Task CargarCombos(int? usuarioSel = null, int? citaSel = null)
        {
            var usuarios = await _api.GetUsuariosAsync();
            var citas = await _api.GetCitasAsync();

            ViewBag.Usuarios = new SelectList(
                usuarios.Select(u => new { u.IdUsuario, Nombre = $"{u.Nombre} {u.Apellido}" }),
                "IdUsuario", "Nombre", usuarioSel);

            ViewBag.Citas = new SelectList(
                citas.Select(c => new { c.IdCita, c.Descripcion }),
                "IdCita", "Descripcion", citaSel);
        }
    }
}

