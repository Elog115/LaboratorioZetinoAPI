using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Data.Repositories;
using SisLabZetino.Infrastructure.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Tests.Functional
{
    [TestClass]
    public class NotificacionEmailServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private NotificacionEmailRepository _repository = null!;
        private NotificacionEmailService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // Cargar configuración desde el proyecto Web
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\LabZetino.Web");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            _context = new AppDBContext(options);
            _repository = new NotificacionEmailRepository(_context);
            _service = new NotificacionEmailService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Agregar una nueva notificación
        [TestMethod]
        public async Task AgregarNotificacionAsync_DeberiaAgregarNotificacionEnBaseDeDatos()
        {
            var notificacion = new NotificacionEmail
            {
                IdResultado = 1,
                Asunto = "Resultado Disponible " + Guid.NewGuid(),
                Mensaje = "Su resultado de laboratorio ya está disponible.",
                EstadoEnvio = "Pendiente",
                Estado = true
            };

            var resultado = await _service.AgregarNotificacionAsync(notificacion);

            Assert.AreEqual("Notificación agregada correctamente", resultado);

            var notificacionGuardada = await _context.NotificacionesEmail
                .FirstOrDefaultAsync(n => n.Asunto == notificacion.Asunto);
            Assert.IsNotNull(notificacionGuardada);
            Assert.AreEqual(true, notificacionGuardada.Estado);
        }

        // 2️⃣ Modificar una notificación existente
        [TestMethod]
        public async Task ModificarNotificacionAsync_DeberiaActualizarElMensaje()
        {
            var notificacion = new NotificacionEmail
            {
                IdResultado = 2,
                Asunto = "Notificación a modificar " + Guid.NewGuid(),
                Mensaje = "Mensaje original.",
                EstadoEnvio = "Pendiente",
                Estado = true
            };
            await _repository.AddNotificacionAsync(notificacion);

            var nuevoMensaje = "Este es el mensaje actualizado " + Guid.NewGuid();
            notificacion.Mensaje = nuevoMensaje;

            var resultado = await _service.ModificarNotificacionAsync(notificacion);

            Assert.AreEqual("Notificación modificada correctamente", resultado);
            var notificacionActualizada = await _context.NotificacionesEmail
                .FirstOrDefaultAsync(n => n.IdNotificacion == notificacion.IdNotificacion);
            Assert.IsNotNull(notificacionActualizada);
            Assert.AreEqual(nuevoMensaje, notificacionActualizada.Mensaje);
        }

        // 3️⃣ Eliminar una notificación (borrado físico)
        [TestMethod]
        public async Task EliminarNotificacionAsync_DeberiaRemoverLaNotificacionDeLaBD()
        {
            var notificacion = new NotificacionEmail
            {
                IdResultado = 3,
                Asunto = "Notificación para eliminar " + Guid.NewGuid(),
                Mensaje = "Este mensaje se eliminará.",
                EstadoEnvio = "Enviado",
                Estado = true
            };
            await _repository.AddNotificacionAsync(notificacion);

            var resultado = await _service.EliminarNotificacionAsync(notificacion.IdNotificacion);

            Assert.AreEqual("Notificación eliminada correctamente", resultado);
            var notificacionEliminada = await _context.NotificacionesEmail
                .FirstOrDefaultAsync(n => n.IdNotificacion == notificacion.IdNotificacion);
            Assert.IsNull(notificacionEliminada);
        }

        // 4️⃣ Cancelar una notificación (borrado lógico)
        [TestMethod]
        public async Task CancelarNotificacionAsync_DeberiaMarcarEstadoComoFalse()
        {
            var notificacion = new NotificacionEmail
            {
                IdResultado = 4,
                Asunto = "Notificación para cancelar " + Guid.NewGuid(),
                Mensaje = "Esta notificación será cancelada.",
                EstadoEnvio = "Pendiente",
                Estado = true
            };
            await _repository.AddNotificacionAsync(notificacion);

            var resultado = await _service.CancelarNotificacionAsync(notificacion.IdNotificacion);

            Assert.AreEqual("Notificación cancelada correctamente", resultado);
            var notificacionCancelada = await _context.NotificacionesEmail
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdNotificacion == notificacion.IdNotificacion);
            Assert.IsNotNull(notificacionCancelada);
            Assert.IsFalse(notificacionCancelada.Estado);
        }

        // 5️⃣ Obtener notificaciones activas
        [TestMethod]
        public async Task ObtenerNotificacionesActivasAsync_DeberiaRetornarSoloActivas()
        {
            var notificacionActiva = new NotificacionEmail
            {
                IdResultado = 5,
                Asunto = "Notificación Activa " + Guid.NewGuid(),
                Mensaje = "Activa",
                EstadoEnvio = "Pendiente",
                Estado = true
            };
            var notificacionInactiva = new NotificacionEmail
            {
                IdResultado = 5,
                Asunto = "Notificación Inactiva " + Guid.NewGuid(),
                Mensaje = "Inactiva",
                EstadoEnvio = "Pendiente",
                Estado = false
            };
            await _repository.AddNotificacionAsync(notificacionActiva);
            await _repository.AddNotificacionAsync(notificacionInactiva);

            var notificaciones = await _service.ObtenerNotificacionesActivasAsync();

            Assert.IsTrue(notificaciones.Any(n => n.IdNotificacion == notificacionActiva.IdNotificacion));
            Assert.IsFalse(notificaciones.Any(n => n.IdNotificacion == notificacionInactiva.IdNotificacion));
        }

        // 6️⃣ Obtener notificaciones por estado de envío
        [TestMethod]
        public async Task ObtenerNotificacionesPorEstadoEnvioAsync_DeberiaRetornarSoloPendientes()
        {
            var estadoBuscado = "PendienteDeEnvio";
            var notificacionPendiente = new NotificacionEmail
            {
                IdResultado = 6,
                Asunto = "Notificación Pendiente " + Guid.NewGuid(),
                Mensaje = "Este es un mensaje de prueba pendiente.",
                EstadoEnvio = estadoBuscado,
                Estado = true
            };
            var notificacionEnviada = new NotificacionEmail
            {
                IdResultado = 6,
                Asunto = "Notificación Enviada " + Guid.NewGuid(),
                Mensaje = "Este es un mensaje de prueba enviado.",
                EstadoEnvio = "EnviadoExitosamente",
                Estado = true
            };
            await _repository.AddNotificacionAsync(notificacionPendiente);
            await _repository.AddNotificacionAsync(notificacionEnviada);

            var resultado = await _service.ObtenerNotificacionesPorEstadoEnvioAsync(estadoBuscado);

            Assert.IsTrue(resultado.Any(n => n.IdNotificacion == notificacionPendiente.IdNotificacion));
            Assert.IsFalse(resultado.Any(n => n.IdNotificacion == notificacionEnviada.IdNotificacion));
        }

        // 7️⃣ Intentar eliminar una notificación que no existe
        [TestMethod]
        public async Task EliminarNotificacionAsync_DeberiaRetornarErrorSiNoExiste()
        {
            var idInexistente = -999999; // Usar un ID que seguramente no exista
            var resultado = await _service.EliminarNotificacionAsync(idInexistente);
            Assert.AreEqual("Error: Notificación no encontrada", resultado);
        }

        // 8️⃣ Obtener notificaciones por resultado

        [TestMethod]
        public async Task ObtenerNotificacionesPorResultadoAsync_DeberiaRetornarSoloNotificacionesDelResultado()
        {
            // 🔹 Generamos un IdResultado único para esta prueba
            var idResultadoBuscado = new Random().Next(1000, 9999);

            var notificacionResultadoCorrecto = new NotificacionEmail
            {
                IdResultado = idResultadoBuscado,
                Asunto = "Resultado Correcto " + Guid.NewGuid(),
                Mensaje = "Mensaje para el resultado correcto.",
                EstadoEnvio = "Pendiente",
                Estado = true
            };

            var notificacionResultadoIncorrecto = new NotificacionEmail
            {
                IdResultado = idResultadoBuscado + 1, // Otro IdResultado
                Asunto = "Resultado Incorrecto " + Guid.NewGuid(),
                Mensaje = "Mensaje para otro resultado.",
                EstadoEnvio = "Pendiente",
                Estado = true
            };

            // 🔹 Insertamos ambas notificaciones
            await _repository.AddNotificacionAsync(notificacionResultadoCorrecto);
            await _repository.AddNotificacionAsync(notificacionResultadoIncorrecto);

            // 🔹 Ejecutamos el método a probar
            var resultado = await _service.ObtenerNotificacionesPorResultadoAsync(idResultadoBuscado);

            // 🔹 Validaciones
            Assert.IsTrue(resultado.Any(n => n.Asunto == notificacionResultadoCorrecto.Asunto),
                "Debe contener la notificación con el IdResultado buscado.");
            Assert.IsFalse(resultado.Any(n => n.Asunto == notificacionResultadoIncorrecto.Asunto),
                "No debe contener notificaciones de otros IdResultado.");
            Assert.AreEqual(1, resultado.Count(), "Debe devolver solo una notificación.");
        }
    }
}

