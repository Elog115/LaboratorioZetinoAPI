using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   // 👈 IMPORTANTE para SelectList
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using System.Threading.Tasks;

// 🔹 Necesarios para QuestPDF y LINQ (para el comprobante)
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class NotificacionEmailController : Controller
    {
        private readonly IApiClient _api;
        private readonly IEmailService _emailService;

        public NotificacionEmailController(IApiClient apiClient, IEmailService emailService)
        {
            _api = apiClient;
            _emailService = emailService;
        }

        // GET: /NotificacionEmail/
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var notificaciones = await _api.GetNotificacionesEmailAsync();
            return View(notificaciones);
        }

        // GET: /NotificacionEmail/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var modelo = new NotificacionEmailDto
            {
                Estado = true,
                EstadoEnvio = "Pendiente"
            };

            // 👇 Cargamos la lista de resultados para el <select>
            await CargarResultadosDropdown();

            return View(modelo);
        }

        // POST: /NotificacionEmail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificacionEmailDto notificacion)
        {
            // 1) Validación de modelo
            if (!ModelState.IsValid)
            {
                await CargarResultadosDropdown(notificacion.IdResultado);
                return View(notificacion);
            }

            // 2) Valores por defecto
            if (string.IsNullOrWhiteSpace(notificacion.EstadoEnvio))
                notificacion.EstadoEnvio = "Pendiente";

            // Por si acaso
            if (!notificacion.Estado)
                notificacion.Estado = true;

            // 3) Llamar a la API
            var respuesta = await _api.CreateNotificacionEmailAsync(notificacion);

            // 4) Analizar respuesta de la API
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                TempData["Error"] = "Error al crear notificación: la API no devolvió respuesta.";
                await CargarResultadosDropdown(notificacion.IdResultado);
                return View(notificacion);
            }

            if (respuesta.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
            {
                // Aquí ya te va a mostrar el detalle que devuelva tu API
                TempData["Error"] = "Error al crear notificación: " + respuesta;
                await CargarResultadosDropdown(notificacion.IdResultado);
                return View(notificacion);
            }

            // 5) Éxito
            TempData["Success"] = "✅ Notificación registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // GET: /NotificacionEmail/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var notificacion = await _api.GetNotificacionEmailAsync(id);
            if (notificacion == null)
            {
                TempData["Error"] = "No se encontró la notificación.";
                return RedirectToAction(nameof(Index));
            }

            notificacion.Estado = !notificacion.Estado;

            var resultado = await _api.UpdateNotificacionEmailAsync(id, notificacion);

            if (resultado.StartsWith("Error"))
            {
                TempData["Error"] = resultado;
            }

            return RedirectToAction(nameof(Index));
        }

        // 🔹 COMPROBANTE PDF DEL RESULTADO
        // GET: /NotificacionEmail/Comprobante/5   (5 = IdResultado)
        [HttpGet]
        public async Task<IActionResult> Comprobante(int idResultado)
        {
            var resultado = await _api.GetResultadoAsync(idResultado);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado para generar el comprobante.";
                return RedirectToAction(nameof(Index));
            }

            var examenes = await _api.GetExamenesAsync();
            var examen = examenes?.FirstOrDefault(e => e.IdExamen == resultado.IdExamen);
            var nombreExamen = examen?.Descripcion ?? $"ID examen: {resultado.IdExamen}";

            QuestPDF.Settings.License = LicenseType.Community;

            var fechaEntregaTexto = resultado.FechaEntrega.ToString("dd/MM/yyyy");
            var estadoTexto = resultado.Estado ? "Activo" : "Inactivo";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A5);

                    page.Header()
                        .Text("Comprobante de Resultado - Laboratorio Zetino")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().Text($"Número de resultado: {resultado.IdResultado}")
                            .SemiBold();

                        col.Item().Text($"Examen: {nombreExamen}");

                        col.Item().Text($"Fecha de entrega: {fechaEntregaTexto}");

                        col.Item().Text("Observaciones:")
                            .SemiBold();

                        col.Item().Text(resultado.Observaciones ?? "Sin observaciones registradas.")
                            .FontSize(11);

                        col.Item().Text($"Estado: {estadoTexto}")
                            .SemiBold()
                            .FontColor(resultado.Estado ? Colors.Green.Darken2 : Colors.Red.Darken2);

                        col.Item().PaddingTop(10)
                            .Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Laboratorio Zetino - Sistema de Resultados")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            var pdfBytes = document.GeneratePdf();
            var fileName = $"Resultado_{resultado.IdResultado}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        // ✅ ENVIAR COMPROBANTE PDF DEL RESULTADO POR CORREO
        // GET: /NotificacionEmail/Enviar/5   (5 = IdResultado)
        [HttpGet]
        public async Task<IActionResult> Enviar(int idResultado)
        {
            var resultado = await _api.GetResultadoAsync(idResultado);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado.";
                return RedirectToAction("Index", "Resultado");
            }

            var examenes = await _api.GetExamenesAsync();
            var examen = examenes?.FirstOrDefault(e => e.IdExamen == resultado.IdExamen);
            var nombreExamen = examen?.Descripcion ?? $"ID examen: {resultado.IdExamen}";

            var usuarios = await _api.GetUsuariosAsync();
            var paciente = usuarios.FirstOrDefault(u => u.IdUsuario == resultado.IdUsuario);

            if (paciente == null)
            {
                TempData["Error"] = "No se encontró el paciente asociado a este resultado.";
                return RedirectToAction("Index", "Resultado");
            }

            if (string.IsNullOrWhiteSpace(paciente.Email))
            {
                TempData["Error"] = "El paciente no tiene un correo electrónico registrado.";
                return RedirectToAction("Index", "Resultado");
            }

            string nombrePaciente = $"{paciente.Nombre} {paciente.Apellido}".Trim();
            string emailPaciente = paciente.Email;

            QuestPDF.Settings.License = LicenseType.Community;

            var fechaEntregaTexto = resultado.FechaEntrega.ToString("dd/MM/yyyy");
            var estadoTexto = resultado.Estado ? "Activo" : "Inactivo";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A5);

                    page.Header()
                        .Text("Comprobante de Resultado - Laboratorio Zetino")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().Text($"Número de resultado: {resultado.IdResultado}")
                            .SemiBold();

                        col.Item().Text($"Examen: {nombreExamen}");

                        col.Item().Text($"Fecha de entrega: {fechaEntregaTexto}");

                        col.Item().Text("Observaciones:")
                            .SemiBold();

                        col.Item().Text(resultado.Observaciones ?? "Sin observaciones registradas.")
                            .FontSize(11);

                        col.Item().Text($"Estado: {estadoTexto}")
                            .SemiBold()
                            .FontColor(resultado.Estado ? Colors.Green.Darken2 : Colors.Red.Darken2);

                        col.Item().PaddingTop(10)
                            .Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Laboratorio Zetino - Sistema de Resultados")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            var pdfBytes = document.GeneratePdf();
            var fileName = $"Resultado_{resultado.IdResultado}.pdf";

            string asunto = $"Resultado de examen - {nombrePaciente}";
            string mensajeHtml = $@"
        <p>Estimado(a) <strong>{nombrePaciente}</strong>,</p>
        <p>Adjuntamos el comprobante de su resultado de examen: <strong>{nombreExamen}</strong>.</p>
        <p>Si tiene alguna duda, puede contactarnos respondiendo a este correo.</p>
        <p>Saludos cordiales,<br/>Laboratorio Zetino</p>
    ";

            var notificacion = new NotificacionEmailDto
            {
                IdResultado = idResultado,
                Asunto = asunto,
                Mensaje = mensajeHtml,
                EstadoEnvio = "Pendiente",
                Estado = true
            };

            try
            {
                await _emailService.SendEmailWithAttachmentAsync(
                    emailPaciente,
                    asunto,
                    mensajeHtml,
                    pdfBytes,
                    fileName
                );

                notificacion.EstadoEnvio = "Enviado";
                TempData["Success"] = "✅ El comprobante del resultado fue enviado correctamente al correo del paciente.";
            }
            catch
            {
                notificacion.EstadoEnvio = "Error";
                TempData["Error"] = "❌ Ocurrió un error al enviar el correo. Verifica la configuración de correo.";
            }

            var respuesta = await _api.CreateNotificacionEmailAsync(notificacion);
            if (respuesta.StartsWith("Error"))
            {
                TempData["Error"] = respuesta;
            }
             
            return RedirectToAction("Index", "Resultado");
        }

        // 🔹 Helper privado para llenar el dropdown de resultados
        private async Task CargarResultadosDropdown(object selectedValue = null)
        {
            var resultados = await _api.GetResultadosAsync(null) ?? Enumerable.Empty<ResultadoDto>();

            // De momento mostramos solo el IdResultado en el combo
            ViewBag.Resultados = new SelectList(
                resultados.OrderBy(r => r.IdResultado),
                "IdResultado",
                "IdResultado",
                selectedValue
            );
        }
    }
}
