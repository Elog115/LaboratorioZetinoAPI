using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoZetino.WebMVC.Models;
using ProyectoZetino.WebMVC.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Controllers
{
    public class ResultadoController : Controller
    {
        private readonly IApiClient _api;

        public ResultadoController(IApiClient apiClient)
        {
            _api = apiClient;
        }

        // GET: /Resultado/
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var resultados = await _api.GetResultadosAsync(searchTerm);

            // Cargar exámenes para mostrar la descripción
            var examenes = await _api.GetExamenesAsync();
            ViewBag.ListaExamenes = examenes ?? new List<ExamenDto>();

            return View(resultados);
        }

        // GET: /Resultado/Create
        public async Task<IActionResult> Create()
        {
            await CargarExamenesDropdown();
            await CargarUsuariosDropdown();              // 🆕 cargar pacientes
            return View();
        }

        // POST: /Resultado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultadoDto resultado)
        {
            if (!ModelState.IsValid)
            {
                await CargarExamenesDropdown(resultado.IdExamen);
                await CargarUsuariosDropdown(resultado.IdUsuario);   // 🆕 recargar pacientes
                return View(resultado);
            }

            resultado.Estado = true;

            var success = await _api.CreateResultadoAsync(resultado);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el resultado.");
            await CargarExamenesDropdown(resultado.IdExamen);
            await CargarUsuariosDropdown(resultado.IdUsuario);       // 🆕 también aquí
            return View(resultado);
        }

        // GET: /Resultado/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
            if (resultado == null)
                return NotFound();

            await CargarExamenesDropdown(resultado.IdExamen);
            await CargarUsuariosDropdown(resultado.IdUsuario);       // 🆕 para el combo en Edit
            return View(resultado);
        }

        // POST: /Resultado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResultadoDto resultado)
        {
            if (id != resultado.IdResultado)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await CargarExamenesDropdown(resultado.IdExamen);
                await CargarUsuariosDropdown(resultado.IdUsuario);   // 🆕 recargar combo
                return View(resultado);
            }

            var success = await _api.UpdateResultadoAsync(id, resultado);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el resultado.");
            await CargarExamenesDropdown(resultado.IdExamen);
            await CargarUsuariosDropdown(resultado.IdUsuario);       // 🆕 también aquí
            return View(resultado);
        }

        // GET: /Resultado/ToggleEstado/5
        [HttpGet]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
            if (resultado == null)
            {
                TempData["Error"] = "No se encontró el resultado.";
                return RedirectToAction(nameof(Index));
            }

            resultado.Estado = !resultado.Estado;
            var success = await _api.UpdateResultadoAsync(id, resultado);

            if (!success)
                TempData["Error"] = "Error al cambiar el estado del resultado.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Resultado/Lista
        [HttpGet]
        public async Task<IActionResult> Lista(string q = null)
        {
            var resultados = await _api.GetResultadosAsync(q);
            var examenes = await _api.GetExamenesAsync();
            var usuarios = await _api.GetUsuariosAsync();
            ViewBag.ListaExamenes = examenes ?? new List<ExamenDto>();
            ViewBag.ListaUsuarios = usuarios ?? new List<UsuarioDto>();
            return PartialView("_TablaResultados", resultados);
        }

        // 🔹 COMPROBANTE PDF DE RESULTADO (QuestPDF - descarga PDF)
        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
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

        // 🔹 COMPROBANTE EN HTML PARA MODAL
        [HttpGet]
        public async Task<IActionResult> ComprobanteHtml(int id)
        {
            var resultado = await _api.GetResultadoAsync(id);
            if (resultado == null)
            {
                return NotFound();
            }

            var examenes = await _api.GetExamenesAsync();
            var examen = examenes?.FirstOrDefault(e => e.IdExamen == resultado.IdExamen);
            ViewBag.NombreExamen = examen?.Descripcion ?? $"ID examen: {resultado.IdExamen}";

            return PartialView("ComprobanteResultado", resultado);
        }

        // --- MÉTODOS HELPER PRIVADOS ---

        private async Task CargarExamenesDropdown(object? selectedValue = null)
        {
            var examenes = await _api.GetExamenesAsync();

            ViewBag.Examenes = new SelectList(
                examenes.Where(e => e.Estado)
                        .OrderBy(e => e.Descripcion),
                "IdExamen",
                "Descripcion",
                selectedValue
            );
        }

        // 🆕 Helper para llenar el combo de pacientes
        private async Task CargarUsuariosDropdown(object? selectedValue = null)
        {
            var usuarios = await _api.GetUsuariosAsync();

            ViewBag.Usuarios = new SelectList(
                usuarios
                    .Where(u => u.Estado)             // si tu DTO tiene Estado
                    .OrderBy(u => u.Nombre),
                "IdUsuario",
                "Nombre",                            // o $"{u.Nombre} {u.Apellido}" si lo tienes
                selectedValue
            );
        }
    }
}
