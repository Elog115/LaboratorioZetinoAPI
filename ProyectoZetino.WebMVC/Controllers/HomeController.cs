using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ProyectoZetino.WebMVC.Models; // (Tu namespace de Modelos)
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ProyectoZetino.WebMVC.Controllers // (Tu namespace de Controladores)
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

        // Esta es la nueva acción que controla tu vista Dashboard.cshtml
        public IActionResult Dashboard()
        {
            // Verificamos si la cookie de autenticación existe
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                // Si alguien intenta entrar a /Home/Dashboard sin cookie, lo echamos
                return RedirectToAction("Login", "Account");
            }

            // Si hay token, le mostramos su Dashboard.
            ViewData["Title"] = "Dashboard";
            return View(); // Esto renderiza /Views/Home/Dashboard.cshtml
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