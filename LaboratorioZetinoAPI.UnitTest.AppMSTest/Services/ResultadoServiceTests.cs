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
    public class ResultadoServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private ResultadoRepository _repository = null!;
        private ResultadoService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // 📂 Cargar configuración desde el proyecto Web
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
            _repository = new ResultadoRepository(_context);
            _service = new ResultadoService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Agregar un nuevo resultado
        [TestMethod]
        public async Task AgregarResultadoAsync_DeberiaAgregarResultadoEnBaseDeDatos()
        {
            var resultado = new Resultado
            {
                IdExamen = 1,
                FechaEntrega = DateTime.Now.AddDays(1),
                Observaciones = "Resultado de prueba " + Guid.NewGuid(),
                ArchivoResultado = "archivo_prueba.pdf",
                Estado = true
            };

            var mensaje = await _service.AgregarResultadoAsync(resultado);

            Assert.AreEqual("Resultado agregado correctamente", mensaje);

            var guardado = await _context.Resultados
                .FirstOrDefaultAsync(r => r.Observaciones == resultado.Observaciones);

            Assert.IsNotNull(guardado);
            Assert.IsTrue(guardado.Estado);
        }

        // 2️⃣ Modificar un resultado existente
        [TestMethod]
        public async Task ModificarResultadoAsync_DeberiaActualizarObservaciones()
        {
            var resultado = new Resultado
            {
                IdExamen = 1,
                FechaEntrega = DateTime.Now.AddDays(2),
                Observaciones = "Resultado original " + Guid.NewGuid(),
                ArchivoResultado = "archivo_original.pdf",
                Estado = true
            };

            await _repository.AddResultadoAsync(resultado);

            var nuevaObservacion = "Resultado modificado " + Guid.NewGuid();
            resultado.Observaciones = nuevaObservacion;

            var mensaje = await _service.ModificarResultadoAsync(resultado);

            Assert.AreEqual("Resultado modificado correctamente", mensaje);

            var actualizado = await _context.Resultados
                .FirstOrDefaultAsync(r => r.IdResultado == resultado.IdResultado);

            Assert.IsNotNull(actualizado);
            Assert.AreEqual(nuevaObservacion, actualizado.Observaciones);
        }

        // 3️⃣ Cancelar resultado (borrado lógico)
        [TestMethod]
        public async Task CancelarResultadoAsync_DeberiaMarcarEstadoFalse()
        {
            var resultado = new Resultado
            {
                IdExamen = 2,
                FechaEntrega = DateTime.Now.AddDays(3),
                Observaciones = "Resultado a cancelar " + Guid.NewGuid(),
                ArchivoResultado = "archivo_cancelar.pdf",
                Estado = true
            };

            await _repository.AddResultadoAsync(resultado);

            var mensaje = await _service.CancelarResultadoAsync(resultado.IdResultado);

            Assert.AreEqual("Resultado cancelado correctamente", mensaje);

            var cancelado = await _context.Resultados
                .FirstOrDefaultAsync(r => r.IdResultado == resultado.IdResultado);

            Assert.IsNotNull(cancelado);
            Assert.IsFalse(cancelado.Estado);
        }

        // 4️⃣ Eliminar resultado (borrado físico)
        [TestMethod]
        public async Task EliminarResultadoAsync_DeberiaMarcarEstadoFalse()
        {
            var resultado = new Resultado
            {
                IdExamen = 3,
                FechaEntrega = DateTime.Now.AddDays(4),
                Observaciones = "Resultado a eliminar " + Guid.NewGuid(),
                ArchivoResultado = "archivo_eliminar.pdf",
                Estado = true
            };

            await _repository.AddResultadoAsync(resultado);

            var mensaje = await _service.EliminarResultadoAsync(resultado.IdResultado);

            Assert.AreEqual("Resultado eliminado correctamente", mensaje);

            var eliminado = await _context.Resultados
                .FirstOrDefaultAsync(r => r.IdResultado == resultado.IdResultado);

            Assert.IsNotNull(eliminado);      // ✅ Sigue existiendo
            Assert.IsFalse(eliminado!.Estado); // ✅ Estado debe ser false
        }


        // 5️⃣ Obtener solo resultados activos
        [TestMethod]
        public async Task ObtenerResultadosActivosAsync_DeberiaRetornarSoloActivos()
        {
            var activo = new Resultado
            {
                IdExamen = 4,
                FechaEntrega = DateTime.Now.AddDays(5),
                Observaciones = "Resultado activo " + Guid.NewGuid(),
                ArchivoResultado = "archivo_test_activo.pdf",
                Estado = true
            };

            var inactivo = new Resultado
            {
                IdExamen = 4,
                FechaEntrega = DateTime.Now.AddDays(6),
                Observaciones = "Resultado inactivo " + Guid.NewGuid(),
                ArchivoResultado = "archivo_test_inactivo.pdf",
                Estado = false
            };

            await _repository.AddResultadoAsync(activo);
            await _repository.AddResultadoAsync(inactivo);

            var activos = await _service.ObtenerResultadosActivosAsync();

            Assert.IsTrue(activos.Any(r => r.IdResultado == activo.IdResultado));
            Assert.IsFalse(activos.Any(r => r.IdResultado == inactivo.IdResultado));
        }

    }
}
