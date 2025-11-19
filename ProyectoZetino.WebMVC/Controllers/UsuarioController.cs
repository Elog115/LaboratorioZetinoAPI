using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IApiClient _api;
        public UsuarioController(IApiClient apiClient) => _api = apiClient;

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
            await CargarRoles();
            return View(new UsuarioDto { Estado = true });
        }

        // POST: /Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto usuario)
        {
            if (!ModelState.IsValid)
            {
                await CargarRoles(usuario.IdRol);
                return View(usuario);
            }

            if (!usuario.FechaNacimiento.HasValue)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento es obligatoria.");
                await CargarRoles(usuario.IdRol);
                return View(usuario);
            }

            usuario.FechaNacimiento = usuario.FechaNacimiento.Value.Date;
            usuario.Estado = true;

            var ok = await _api.CreateUsuarioAsync(usuario);
            if (ok)
            {
                TempData["Success"] = $"✅ Usuario '{usuario.Nombre}' creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo crear el usuario.";
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

            await CargarRoles(usuario.IdRol);
            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioDto usuario)
        {
            if (id != usuario.IdUsuario)
            {
                TempData["Error"] = "❌ Identificador inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CargarRoles(usuario.IdRol);
                return View(usuario);
            }

            var ok = await _api.UpdateUsuarioAsync(id, usuario);
            if (ok)
            {
                TempData["Success"] = $"✅ Usuario '{usuario.Nombre}' actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo actualizar el usuario.";
            await CargarRoles(usuario.IdRol);
            return View(usuario);
        }

        // GET: /Usuario/Delete/5  → Confirmación
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _api.GetUsuarioByIdAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: /Usuario/Delete/5  → Ejecuta eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _api.DeleteUsuarioAsync(id);
            if (ok)
            {
                TempData["Success"] = "🗑️ Usuario eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ No se pudo eliminar el usuario.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var ok = await _api.ToggleEstadoUsuarioAsync(id);

            if (!ok)
                TempData["Error"] = "❌ No se pudo cambiar el estado del usuario.";

            return RedirectToAction(nameof(Index));
        }


        // 🔹 Helper para cargar roles
        private async Task CargarRoles(int? seleccionado = null)
        {
            var roles = await _api.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre", seleccionado);
        }

    }
}
