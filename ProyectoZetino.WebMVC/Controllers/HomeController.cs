using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ProyectoZetino.WebMVC.Models; // (Tu namespace de Modelos)
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

// ⭐ Agregamos el namespace donde está IApiClient
using ProyectoZetino.WebMVC.Services;

namespace ProyectoZetino.WebMVC.Controllers // (Tu namespace de Controladores)
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // ⭐ NUEVO: cliente para llamar a la API
        private readonly IApiClient _api;

        // ⭐ Solo agregamos el parámetro IApiClient, lo demás queda igual
        public HomeController(
            ILogger<HomeController> logger,
            IHttpContextAccessor httpContextAccessor,
            IApiClient api)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _api = api;
        }

        public IActionResult Index()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // Lo redirigimos al NUEVO Dashboard, no a Roles.
                return RedirectToAction("Dashboard", "Home");
            }

            // Si no hay token, muestra la Landing Page
            return View();
        }

        // ⭐ Hacemos Dashboard async para poder llamar a la API
        public async Task<IActionResult> Dashboard()
        {
            // Verificamos si la cookie de autenticación existe
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Traer citas desde la API
            var citas = await _api.GetCitasAsync(null);

            // 👇 Versión sencilla (FUNCIONA SIEMPRE, sin usar propiedad Activas todavía)
            int totalCitas = citas?.Count() ?? 0;
            int activas = totalCitas;   // por ahora asumimos todas activas
            int inactivas = 0;          // por ahora ninguna inactiva

            ViewBag.CitasActivas = activas;
            ViewBag.CitasInactivas = inactivas;

            ViewData["Title"] = "Dashboard";
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
