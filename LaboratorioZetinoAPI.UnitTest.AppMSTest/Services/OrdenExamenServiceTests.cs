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
    public class OrdenExamenServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private OrdenExamenRepository _repository = null!;
        private OrdenExamenService _service = null!;
        private CitaRepository _citaRepository = null!; // Necesario para crear citas de prueba

        [TestInitialize]
        public void Setup()
        {
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
            _repository = new OrdenExamenRepository(_context);
            _service = new OrdenExamenService(_repository);
            _citaRepository = new CitaRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // Método auxiliar para crear una cita y asegurar que la FK exista
        private async Task<Cita> CrearCitaDePruebaAsync(int idUsuario)
        {
            var cita = new Cita
            {
                IdUsuario = idUsuario,
                FechaHora = DateTime.Now.AddDays(20).AddHours(DateTime.Now.Minute),
                Descripcion = "Cita para orden de examen " + Guid.NewGuid(),
                Estado = true
            };
            await _citaRepository.AddCitaAsync(cita);
            return cita;
        }

        // 1️⃣ Agregar una nueva orden
        [TestMethod]
        public async Task AgregarOrdenAsync_DeberiaAgregarOrdenEnBaseDeDatos()
        {
            var cita = await CrearCitaDePruebaAsync(10);
            var orden = new OrdenExamen
            {
                IdUsuario = cita.IdUsuario,
                IdCita = cita.IdCita,
                FechaSolicitud = DateTime.Now.Date,
                Estado = true
            };

            var resultado = await _service.AgregarOrdenAsync(orden);

            Assert.AreEqual("Orden agregada correctamente", resultado);
            var ordenGuardada = await _context.OrdenesExamen
                .FirstOrDefaultAsync(o => o.IdOrdenExamen == orden.IdOrdenExamen);
            Assert.IsNotNull(ordenGuardada);
            Assert.AreEqual(cita.IdCita, ordenGuardada.IdCita);
        }

        // 2️⃣ Modificar una orden existente
        [TestMethod]
        public async Task ModificarOrdenAsync_DeberiaActualizarElEstado()
        {
            var cita = await CrearCitaDePruebaAsync(11);
            var orden = new OrdenExamen
            {
                IdUsuario = cita.IdUsuario,
                IdCita = cita.IdCita,
                FechaSolicitud = DateTime.Now.Date,
                Estado = true
            };
            await _repository.AddOrdenExamenAsync(orden);

            orden.Estado = false; // Cambiamos el estado
            var resultado = await _service.ModificarOrdenAsync(orden);

            Assert.AreEqual("Orden modificada correctamente", resultado);
            var ordenActualizada = await _context.OrdenesExamen
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.IdOrdenExamen == orden.IdOrdenExamen);
            Assert.IsNotNull(ordenActualizada);
            Assert.IsFalse(ordenActualizada.Estado);
        }

        // 3️⃣ Eliminar una orden (borrado físico)
        [TestMethod]
        public async Task EliminarOrdenAsync_DeberiaRemoverLaOrdenDeLaBD()
        {
            var cita = await CrearCitaDePruebaAsync(12);
            var orden = new OrdenExamen
            {
                IdUsuario = cita.IdUsuario,
                IdCita = cita.IdCita,
                FechaSolicitud = DateTime.Now.Date,
                Estado = true
            };
            await _repository.AddOrdenExamenAsync(orden);

            var resultado = await _service.EliminarOrdenAsync(orden.IdOrdenExamen);

            Assert.AreEqual("Orden eliminada correctamente", resultado);
            var ordenEliminada = await _context.OrdenesExamen
                .FirstOrDefaultAsync(o => o.IdOrdenExamen == orden.IdOrdenExamen);
            Assert.IsNull(ordenEliminada);
        }

        // 4️⃣ Cancelar una orden (borrado lógico)
        [TestMethod]
        public async Task CancelarOrdenAsync_DeberiaMarcarEstadoComoFalse()
        {
            var cita = await CrearCitaDePruebaAsync(13);
            var orden = new OrdenExamen
            {
                IdUsuario = cita.IdUsuario,
                IdCita = cita.IdCita,
                FechaSolicitud = DateTime.Now.Date,
                Estado = true
            };
            await _repository.AddOrdenExamenAsync(orden);

            var resultado = await _service.CancelarOrdenAsync(orden.IdOrdenExamen);

            Assert.AreEqual("Orden cancelada correctamente", resultado);
            var ordenCancelada = await _context.OrdenesExamen
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.IdOrdenExamen == orden.IdOrdenExamen);
            Assert.IsNotNull(ordenCancelada);
            Assert.IsFalse(ordenCancelada.Estado);
        }

        // 5️⃣ Obtener órdenes activas
        [TestMethod]
        public async Task ObtenerOrdenesActivasAsync_DeberiaRetornarSoloActivas()
        {
            var citaActiva = await CrearCitaDePruebaAsync(14);
            var citaInactiva = await CrearCitaDePruebaAsync(14);
            var ordenActiva = new OrdenExamen { IdUsuario = citaActiva.IdUsuario, IdCita = citaActiva.IdCita, FechaSolicitud = DateTime.Now.Date, Estado = true };
            var ordenInactiva = new OrdenExamen { IdUsuario = citaInactiva.IdUsuario, IdCita = citaInactiva.IdCita, FechaSolicitud = DateTime.Now.Date, Estado = false };
            await _repository.AddOrdenExamenAsync(ordenActiva);
            await _repository.AddOrdenExamenAsync(ordenInactiva);

            var ordenes = await _service.ObtenerOrdenesActivasAsync();

            Assert.IsTrue(ordenes.Any(o => o.IdOrdenExamen == ordenActiva.IdOrdenExamen));
            Assert.IsFalse(ordenes.Any(o => o.IdOrdenExamen == ordenInactiva.IdOrdenExamen));
        }

        // 6️⃣ Obtener órdenes por usuario
        [TestMethod]
        public async Task ObtenerOrdenesPorUsuarioAsync_DeberiaRetornarSoloOrdenesDelUsuario()
        {
            var idUsuarioPrueba = 15;
            var cita1 = await CrearCitaDePruebaAsync(idUsuarioPrueba);
            var cita2 = await CrearCitaDePruebaAsync(idUsuarioPrueba + 1); // Otro usuario

            var ordenUsuario1 = new OrdenExamen { IdUsuario = idUsuarioPrueba, IdCita = cita1.IdCita, FechaSolicitud = DateTime.Now.Date, Estado = true };
            var ordenUsuario2 = new OrdenExamen { IdUsuario = idUsuarioPrueba + 1, IdCita = cita2.IdCita, FechaSolicitud = DateTime.Now.Date, Estado = true };
            await _repository.AddOrdenExamenAsync(ordenUsuario1);
            await _repository.AddOrdenExamenAsync(ordenUsuario2);

            var resultado = await _service.ObtenerOrdenesPorUsuarioAsync(idUsuarioPrueba);

            Assert.AreEqual(1, resultado.Count());
            Assert.IsTrue(resultado.All(o => o.IdUsuario == idUsuarioPrueba));
        }

        // 7️⃣ Obtener órdenes por fecha de solicitud
        [TestMethod]
        public async Task ObtenerOrdenesPorFechaSolicitudAsync_DeberiaRetornarSoloOrdenesDeEsaFecha()
        {
            var fechaBuscada = new DateTime(2025, 10, 20);
            var cita1 = await CrearCitaDePruebaAsync(16);
            var cita2 = await CrearCitaDePruebaAsync(16);

            var ordenFechaCorrecta = new OrdenExamen { IdUsuario = 16, IdCita = cita1.IdCita, FechaSolicitud = fechaBuscada, Estado = true };
            var ordenFechaIncorrecta = new OrdenExamen { IdUsuario = 16, IdCita = cita2.IdCita, FechaSolicitud = fechaBuscada.AddDays(1), Estado = true };
            await _repository.AddOrdenExamenAsync(ordenFechaCorrecta);
            await _repository.AddOrdenExamenAsync(ordenFechaIncorrecta);

            var resultado = await _service.ObtenerOrdenesPorFechaSolicitudAsync(fechaBuscada);

            Assert.IsTrue(resultado.Any(o => o.IdOrdenExamen == ordenFechaCorrecta.IdOrdenExamen));
            Assert.IsFalse(resultado.Any(o => o.IdOrdenExamen == ordenFechaIncorrecta.IdOrdenExamen));
        }

        // 8️⃣ Intentar modificar una orden que no existe
        [TestMethod]
        public async Task ModificarOrdenAsync_DeberiaRetornarErrorSiNoExiste()
        {
            var ordenInexistente = new OrdenExamen
            {
                IdOrdenExamen = 999999,
                IdUsuario = 1,
                IdCita = 1,
                FechaSolicitud = DateTime.Now,
                Estado = true
            };

            var resultado = await _service.ModificarOrdenAsync(ordenInexistente);

            Assert.AreEqual("Error: Orden no encontrada", resultado);
        }
    }
}
