using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
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

        // POST: /Cita/Create  (con límite de 35 citas por día)
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

            // 🔹 Validar límite de 35 citas por día
            var fechaSeleccionada = cita.FechaHora.Value.Date;

            // Traemos todas las citas desde la API
            var todasLasCitas = await _api.GetCitasAsync(null); // o "" si tu método lo usa así

            var cantidadCitasMismoDia = todasLasCitas
                .Count(c => c.FechaHora.HasValue && c.FechaHora.Value.Date == fechaSeleccionada);

            if (cantidadCitasMismoDia >= 2) // aquí tú puedes ajustar a 35
            {
                ModelState.AddModelError(string.Empty,
                    $"Ya se alcanzó el límite de 35 citas para el día {fechaSeleccionada:dd/MM/yyyy}. Por favor seleccione otra fecha.");
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

        // GET: /Cita/Comprobante/5  -> Genera comprobante PDF de la cita (QuestPDF)
        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            // 1. Obtener la cita
            var cita = await _api.GetCitaAsync(id);
            if (cita == null)
            {
                TempData["Error"] = "No se encontró la cita para generar el comprobante.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Obtener el paciente (por si NombreUsuario viene vacío)
            var usuario = await _api.GetUsuarioAsync(cita.IdUsuario);
            var nombrePaciente = usuario != null
                ? $"{usuario.Nombre} {usuario.Apellido}"
                : (cita.NombreUsuario ?? "Paciente no especificado");

            // 3. Configuración básica de QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var fechaTexto = cita.FechaHora?.ToString("dd/MM/yyyy HH:mm") ?? "Sin fecha";
            var estadoTexto = cita.Estado ? "Activa" : "Cancelada";

            // 4. Crear el documento PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A5);

                    page.Header()
                        .Text("Comprobante de Cita - Laboratorio Zetino")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().Text($"Número de cita: {cita.IdCita}")
                            .SemiBold();

                        col.Item().Text($"Paciente: {nombrePaciente}");

                        col.Item().Text($"Fecha y hora: {fechaTexto}");

                        col.Item().Text($"Motivo / Descripción:")
                            .SemiBold();

                        col.Item().Text(cita.Descripcion)
                            .FontSize(11);

                        col.Item().Text($"Estado: {estadoTexto}")
                            .SemiBold()
                            .FontColor(cita.Estado ? Colors.Green.Darken2 : Colors.Red.Darken2);

                        col.Item().PaddingTop(10).Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Laboratorio Zetino - Sistema de Citas")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            // 5. Generar el PDF en memoria
            var pdfBytes = document.GeneratePdf();

            var fileName = $"Cita_{cita.IdCita}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // GET: /Cita/ComprobanteHtml/5  -> Comprobante en HTML para el modal
        [HttpGet]
        public async Task<IActionResult> ComprobanteHtml(int id)
        {
            // 1. Obtener la cita
            var cita = await _api.GetCitaAsync(id);
            if (cita == null)
                return NotFound();

            // 2. Obtener el paciente (por si NombreUsuario viene vacío)
            var usuario = await _api.GetUsuarioAsync(cita.IdUsuario);
            var nombrePaciente = usuario != null
                ? $"{usuario.Nombre} {usuario.Apellido}"
                : (cita.NombreUsuario ?? "Paciente no especificado");

            ViewBag.NombrePaciente = nombrePaciente;

            // 3. Devolver solo el HTML del comprobante para el modal
            //    (asegúrate que la vista se llame Views/Cita/ComprobanteCita.cshtml)
            return PartialView("ComprobanteCita", cita);
        }
    }
}
 