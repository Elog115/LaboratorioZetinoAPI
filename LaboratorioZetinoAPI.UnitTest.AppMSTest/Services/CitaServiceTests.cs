using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Repositories;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SisLabZetino.Tests.Functional
{
    [TestClass]
    public class CitaServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private CitaRepository _repository = null!;
        private CitaService _service = null!;

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
            _repository = new CitaRepository(_context);
            _service = new CitaService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Agregar una nueva cita
        [TestMethod]
        public async Task AgregarCitaAsync_DeberiaAgregarCitaEnBaseDeDatos()
        {
            var cita = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(1),
                Descripcion = "Cita de prueba " + Guid.NewGuid(),
                Estado = true
            };

            var resultado = await _service.AgregarCitaAsync(cita);

            Assert.AreEqual("Cita agregada correctamente", resultado);

            var citaGuardada = await _context.Citas
                .FirstOrDefaultAsync(c => c.Descripcion == cita.Descripcion);
            Assert.IsNotNull(citaGuardada);
            Assert.AreEqual(true, citaGuardada.Estado);
        }

        // 2️⃣ Modificar una cita existente
        [TestMethod]
        public async Task ModificarCitaAsync_DeberiaActualizarDescripcion()
        {
            var cita = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(2),
                Descripcion = "Cita a modificar " + Guid.NewGuid(),
                Estado = true
            };

            await _repository.AddCitaAsync(cita);
            var nuevaDescripcion = "Cita modificada " + Guid.NewGuid();
            cita.Descripcion = nuevaDescripcion;

            var resultado = await _service.ModificarCitaAsync(cita);

            Assert.AreEqual("Cita modificada correctamente", resultado);

            var citaActualizada = await _context.Citas
                .FirstOrDefaultAsync(c => c.IdCita == cita.IdCita);
            Assert.IsNotNull(citaActualizada);
            Assert.AreEqual(nuevaDescripcion, citaActualizada.Descripcion);
        }

        // 3️⃣ Cancelar (borrado lógico)
        [TestMethod]
        public async Task CancelarCitaAsync_DeberiaMarcarEstadoFalse()
        {
            var cita = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(3),
                Descripcion = "Cita para cancelar " + Guid.NewGuid(),
                Estado = true
            };

            await _repository.AddCitaAsync(cita);

            var resultado = await _service.CancelarCitaAsync(cita.IdCita);

            Assert.AreEqual("Cita cancelada correctamente", resultado);

            var citaCancelada = await _context.Citas.FirstOrDefaultAsync(c => c.IdCita == cita.IdCita);
            Assert.IsNotNull(citaCancelada);
            Assert.AreEqual(false, citaCancelada.Estado);
        }

        // 4️⃣ Eliminar cita (borrado lógico)
        [TestMethod]
        public async Task EliminarCitaAsync_DeberiaMarcarEstadoFalse()
        {
            var cita = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(4),
                Descripcion = "Cita para eliminar " + Guid.NewGuid(),
                Estado = true
            };

            await _repository.AddCitaAsync(cita);

            var resultado = await _service.EliminarCitaAsync(cita.IdCita);

            Assert.AreEqual("Cita eliminada correctamente", resultado);

            var citaEliminada = await _context.Citas.FirstOrDefaultAsync(c => c.IdCita == cita.IdCita);
            Assert.IsNotNull(citaEliminada);
            Assert.AreEqual(false, citaEliminada.Estado);
        }

        // 5️⃣ Obtener citas activas
        [TestMethod]
        public async Task ObtenerCitasActivasAsync_DeberiaRetornarSoloActivas()
        {
            var citaActiva = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(5),
                Descripcion = "Cita activa " + Guid.NewGuid(),
                Estado = true
            };

            var citaInactiva = new Cita
            {
                IdUsuario = 1,
                FechaHora = DateTime.Now.AddDays(6),
                Descripcion = "Cita inactiva " + Guid.NewGuid(),
                Estado = false
            };

            await _repository.AddCitaAsync(citaActiva);
            await _repository.AddCitaAsync(citaInactiva);

            var citasActivas = await _service.ObtenerCitasActivasAsync();

            Assert.IsTrue(citasActivas.Any(c => c.IdCita == citaActiva.IdCita));
            Assert.IsFalse(citasActivas.Any(c => c.IdCita == citaInactiva.IdCita));
        }
    }
}
