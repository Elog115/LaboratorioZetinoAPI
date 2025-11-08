using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class RolController : Controller
    {
        private readonly IApiClient _api;

        public RolController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /Rol/
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Guardamos el filtro para que el <input> lo muestre
            ViewData["CurrentFilter"] = searchTerm;

            // Llamamos a la API CON el término de búsqueda
            var roles = await _api.GetRolesAsync(searchTerm);

            return View(roles);
        }

        // GET: /Rol/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Rol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolDto rol)
        {
            if (!ModelState.IsValid)
                return View(rol);

            rol.Estado = true;

            var success = await _api.CreateRolAsync(rol);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el rol.");
            return View(rol);
        }

        // GET: /Rol/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var rol = await _api.GetRolAsync(id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        // POST: /Rol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RolDto rol)
        {
            if (id != rol.IdRol)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(rol);

            var success = await _api.UpdateRolAsync(id, rol);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el rol.");
            return View(rol);
        }

        // GET: /Rol/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var rol = await _api.GetRolAsync(id);
            if (rol == null)
            {
                TempData["Error"] = "No se encontró el rol.";
                return RedirectToAction(nameof(Index));
            }

            rol.Estado = !rol.Estado;
            var success = await _api.UpdateRolAsync(id, rol);

            if (!success)
            {
                TempData["Error"] = "Error al cambiar el estado del rol.";
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: /Rol/Lista  -> devuelve solo la tabla para AJAX
        [HttpGet]
        public async Task<IActionResult> Lista(string q = null)
        {
            var roles = await _api.GetRolesAsync(q);
            return PartialView("_TablaRoles", roles);   // <- partial con la tabla
        }

    }
}