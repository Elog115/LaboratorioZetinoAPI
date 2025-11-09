using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IApiClient _api;

        public UsuarioController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /Usuario/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var usuarios = await _api.GetUsuariosAsync(searchTerm);
            return View(usuarios);
        }

        // GET: /Usuario/Create
        public async Task<IActionResult> Create()
        {
            var roles = await _api.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre");
            return View();
        }

        // POST: /Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto usuario)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _api.GetRolesAsync();
                ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre", usuario.IdRol);
                return View(usuario);
            }

            if (usuario.FechaNacimiento.HasValue)
                usuario.FechaNacimiento = usuario.FechaNacimiento.Value.Date;
            else
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento es obligatoria.");
                var roles = await _api.GetRolesAsync();
                ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre", usuario.IdRol);
                return View(usuario);
            }

            usuario.Estado = true;

            var success = await _api.CreateUsuarioAsync(usuario);

            if (success)
            {
                TempData["Success"] = $"✅ Usuario '{usuario.Nombre}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al crear el usuario. La API no pudo guardar el registro.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Usuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _api.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                TempData["Error"] = "❌ Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _api.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre", usuario.IdRol);

            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioDto usuario)
        {
            if (id != usuario.IdUsuario)
            {
                TempData["Error"] = "❌ Error en el identificador del usuario.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var roles = await _api.GetRolesAsync();
                ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre", usuario.IdRol);
                return View(usuario);
            }

            var success = await _api.UpdateUsuarioAsync(id, usuario);
            if (success)
            {
                TempData["Success"] = $"✅ Usuario '{usuario.Nombre}' actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar el usuario.";
            return View(usuario);
        }
    }
}
