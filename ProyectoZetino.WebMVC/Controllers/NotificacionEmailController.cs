using Microsoft.AspNetCore.Mvc;
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



        // 🔹 NUEVO MÉTODO: COMPROBANTE PDF DEL RESULTADO (MISMO DISEÑO QUE EN ResultadoController)
        // GET: /NotificacionEmail/Comprobante/5   (5 = IdResultado)
        [HttpGet]
        public async Task<IActionResult> Comprobante(int idResultado)
        {
            // 1. Obtener el resultado por IdResultado
            var resultado = await _api.GetResultadoAsync(idResultado);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado para generar el comprobante.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Obtener el examen para mostrar el nombre
            var examenes = await _api.GetExamenesAsync();
            var examen = examenes?.FirstOrDefault(e => e.IdExamen == resultado.IdExamen);
            var nombreExamen = examen?.Descripcion ?? $"ID examen: {resultado.IdExamen}";

            // 3. Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var fechaEntregaTexto = resultado.FechaEntrega.ToString("dd/MM/yyyy");
            var estadoTexto = resultado.Estado ? "Activo" : "Inactivo";

            // 4. Crear el documento PDF (misma estructura que usas en ResultadoController)
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

            // 5. Generar PDF en memoria
            var pdfBytes = document.GeneratePdf();
            var fileName = $"Resultado_{resultado.IdResultado}.pdf";

            // 6. Retornar el archivo PDF
            return File(pdfBytes, "application/pdf", fileName);
        }
        // ✅ ENVIAR COMPROBANTE PDF DEL RESULTADO POR CORREO
        // GET: /NotificacionEmail/Enviar/5   (5 = IdResultado)
        [HttpGet]
        public async Task<IActionResult> Enviar(int idResultado)
        {
            // 1. Obtener el resultado
            var resultado = await _api.GetResultadoAsync(idResultado);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado.";
                return RedirectToAction("Index", "Resultado");
            }

            // 2. Obtener el examen para el nombre
            var examenes = await _api.GetExamenesAsync();
            var examen = examenes?.FirstOrDefault(e => e.IdExamen == resultado.IdExamen);
            var nombreExamen = examen?.Descripcion ?? $"ID examen: {resultado.IdExamen}";

            // 3. Obtener el paciente desde la API de usuarios usando IdUsuario
            var usuarios = await _api.GetUsuariosAsync();
            var paciente = usuarios.FirstOrDefault(u => u.IdUsuario == resultado.IdUsuario);

            if (paciente == null)
            {
                TempData["Error"] = "No se encontró el paciente asociado a este resultado.";
                return RedirectToAction("Index", "Resultado");
            }

            if (string.IsNullOrWhiteSpace(paciente.Email)) // ajusta si tu propiedad se llama distinto
            {
                TempData["Error"] = "El paciente no tiene un correo electrónico registrado.";
                return RedirectToAction("Index", "Resultado");
            }

            string nombrePaciente = $"{paciente.Nombre} {paciente.Apellido}".Trim();
            string emailPaciente = paciente.Email;

            // 4. Generar el PDF (igual que en Comprobante)
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

            // 5. Asunto y cuerpo del correo
            string asunto = $"Resultado de examen - {nombrePaciente}";
            string mensajeHtml = $@"
        <p>Estimado(a) <strong>{nombrePaciente}</strong>,</p>
        <p>Adjuntamos el comprobante de su resultado de examen: <strong>{nombreExamen}</strong>.</p>
        <p>Si tiene alguna duda, puede contactarnos respondiendo a este correo.</p>
        <p>Saludos cordiales,<br/>Laboratorio Zetino</p>
    ";

            // 6. Crear registro de notificación
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
                // 7. Enviar correo con adjunto
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

            // 8. Guardar notificación en la API
            var respuesta = await _api.CreateNotificacionEmailAsync(notificacion);
            if (respuesta.StartsWith("Error"))
            {
                TempData["Error"] = respuesta;
            }

            return RedirectToAction("Index", "Resultado");
        }

    }
}
