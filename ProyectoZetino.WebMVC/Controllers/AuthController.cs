using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // Asegúrate de que este using esté aquí

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
            // 💡 Nota: Asegúrate de que IdRol y Nombre existan en el modelo de roles
            ViewBag.Roles = new SelectList(roles, "IdRol", "Nombre");

            return View();
        }

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

            // ✅ Normalizamos la fecha ANTES de enviarla a la API
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
    }
}
