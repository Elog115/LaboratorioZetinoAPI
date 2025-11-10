using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class CitaController : Controller
    {
        private readonly IApiClient _api;
        public CitaController(IApiClient apiClient) => _api = apiClient;

        // GET: /Cita/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var citas = (await _api.GetCitasAsync(searchTerm)).ToList();
            var usuarios = await _api.GetUsuariosAsync(); // Trae todos los usuarios

            // Mapea los nombres de usuario al DTO de cita
            var mapaUsuarios = usuarios.ToDictionary(u => u.IdUsuario,
                                                     u => $"{u.Nombre} {u.Apellido}");

            foreach (var c in citas)
            {
                if (mapaUsuarios.TryGetValue(c.IdUsuario, out var nombre))
                    c.NombreUsuario = nombre;
                else
                    c.NombreUsuario = "Desconocido";
            }

            return View(citas);
        }

        // GET: /Cita/Create
        public async Task<IActionResult> Create()
        {
            await CargarUsuarios();
            return View(new CitaDto
            {
                Estado = true,
                FechaHora = System.DateTime.Now.AddHours(1)
            });
        }

        // POST: /Cita/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaDto cita)
        {
            if (!ModelState.IsValid)
            {
                await CargarUsuarios(cita.IdUsuario);
                return View(cita);
            }

            if (!cita.FechaHora.HasValue)
            {
                ModelState.AddModelError("FechaHora", "La fecha y hora son obligatorias.");
                await CargarUsuarios(cita.IdUsuario);
                return View(cita);
            }

            cita.FechaHora = cita.FechaHora.Value;
            cita.Estado = true;

            var ok = await _api.CreateCitaAsync(cita);
            if (ok)
            {
                TempData["Success"] = "✅ Cita creada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al crear la cita.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cita/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cita = await _api.GetCitaAsync(id);
            if (cita == null)
            {
                TempData["Error"] = "❌ Cita no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            await CargarUsuarios(cita.IdUsuario);
            return View(cita);
        }

        // POST: /Cita/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CitaDto cita)
        {
            if (id != cita.IdCita)
            {
                TempData["Error"] = "❌ Id inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CargarUsuarios(cita.IdUsuario);
                return View(cita);
            }

            var ok = await _api.UpdateCitaAsync(id, cita);
            if (ok)
            {
                TempData["Success"] = "✅ Cita actualizada.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar la cita.";
            await CargarUsuarios(cita.IdUsuario);
            return View(cita);
        }

        // GET: /Cita/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var cita = await _api.GetCitaAsync(id);
            if (cita == null)
            {
                TempData["Error"] = "❌ No se encontró la cita.";
                return RedirectToAction(nameof(Index));
            }

            cita.Estado = !cita.Estado;

            var ok = await _api.UpdateCitaAsync(id, cita);
            if (!ok)
                TempData["Error"] = "❌ No se pudo cambiar el estado de la cita.";

            return RedirectToAction(nameof(Index));
        }

        // Helper para combo de usuarios
        private async Task CargarUsuarios(int? seleccionado = null)
        {
            var usuarios = await _api.GetUsuariosAsync();
            var items = usuarios.Select(u => new
            {
                Id = u.IdUsuario,
                Nombre = $"{u.Nombre} {u.Apellido}"
            });
            ViewBag.Usuarios = new SelectList(items, "Id", "Nombre", seleccionado);
        }
    }
}
