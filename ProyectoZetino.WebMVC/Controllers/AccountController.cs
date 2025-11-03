using Microsoft.AspNetCore.Mvc;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiClient _apiClient;

        // Constructor que inyecta el ApiClient
        public AccountController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // --- LOGIN ---

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está logueado, que lo mande a Roles
            if (Request.Cookies.ContainsKey("AuthToken"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Buena práctica para formularios
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Llama a la API para intentar loguearse
            var token = await _apiClient.LoginAsync(model.Email, model.Password);

            if (token != null)
            {
                // ¡Éxito! Guardamos el token en una cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // El script del navegador no puede leerla
                    Secure = true, // Solo enviar por HTTPS
                    Expires = DateTime.UtcNow.AddHours(8) // Duración de la sesión
                };
                Response.Cookies.Append("AuthToken", token, cookieOptions);


                return RedirectToAction("Index", "Home");
            }

            // Si el token es null, el login falló
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View(model);
        }

        // --- REGISTRO ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Llama a la API para registrar
            bool exito = await _apiClient.RegisterAsync(model);

            if (exito)
            {
                // Si el registro fue exitoso, lo mandamos al Login
                return RedirectToAction("Login");
            }

            // Si el registro falló (ej: email ya existe)
            ModelState.AddModelError(string.Empty, "Error en el registro. Es posible que el email ya esté en uso.");
            return View(model);
        }

        // --- LOGOUT ---

        [HttpPost]
        public IActionResult Logout()
        {
            // Borra la cookie de autenticación
            if (Request.Cookies.ContainsKey("AuthToken"))
            {
                Response.Cookies.Delete("AuthToken");
            }
            // Lo manda al Login
            return RedirectToAction("Login", "Account");
        }
    }
}