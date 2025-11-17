using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class NotificacionEmailController : Controller
    {
        private readonly IApiClient _api;

        public NotificacionEmailController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /NotificacionEmail/
        public async Task<IActionResult> Index(string searchTerm)
        {
            // El filtro JS client-side usará esto
            ViewData["CurrentFilter"] = searchTerm;

            // La API no usa 'searchTerm', así que llamamos sin él
            var notificaciones = await _api.GetNotificacionesEmailAsync();
            return View(notificaciones);
        }

        // GET: /NotificacionEmail/ToggleEstado/5
        // (Para el "borrado lógico" o cancelación)
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var notificacion = await _api.GetNotificacionEmailAsync(id);
            if (notificacion == null)
            {
                TempData["Error"] = "No se encontró la notificación.";
                return RedirectToAction(nameof(Index));
            }

            // Invertimos el estado
            notificacion.Estado = !notificacion.Estado;

            var resultado = await _api.UpdateNotificacionEmailAsync(id, notificacion);

            if (resultado.StartsWith("Error"))
            {
                TempData["Error"] = resultado;
            }

            return RedirectToAction(nameof(Index));
        }

        // NO HAY ACCIONES CREATE O EDIT
    }
}