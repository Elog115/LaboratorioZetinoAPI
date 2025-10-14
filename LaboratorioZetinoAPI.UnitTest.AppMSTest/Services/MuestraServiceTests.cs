using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Data.Repositories;
using SisLabZetino.Infrastructure.Repositories;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Tests.Functional
{
    [TestClass]
    public class MuestraServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private MuestraRepository _repository = null!;
        private MuestraService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // 🔹 Ruta hacia el proyecto web donde está el appsettings.json
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
            _repository = new MuestraRepository(_context);
            _service = new MuestraService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Prueba: Agregar una nueva muestra
        [TestMethod]
        public async Task AgregarMuestraAsync_DeberiaAgregarEnBaseDeDatos()
        {
            // Arrange
            var muestra = new Muestra
            {
                IdOrdenExamen = 1,
                IdTipoMuestra = 1,
                Estado = true,
                FechaRecepcion = System.DateTime.Now
            };

            // Act
            var resultado = await _repository.AddMuestraAsync(muestra);

            // Assert
            var muestraGuardada = await _context.Muestras.FirstOrDefaultAsync(m => m.IdMuestra == resultado.IdMuestra);
            Assert.IsNotNull(muestraGuardada);
            Assert.AreEqual(true, muestraGuardada.Estado);
        }

        // 2️⃣ Prueba: Modificar muestra existente
        [TestMethod]
        public async Task ModificarMuestraAsync_DeberiaActualizarEstado()
        {
            // Arrange
            var muestra = new Muestra
            {
                IdOrdenExamen = 1,
                IdTipoMuestra = 1,
                Estado = true,
                FechaRecepcion = System.DateTime.Now
            };

            await _repository.AddMuestraAsync(muestra);
            muestra.Estado = false;

            // Act
            await _repository.UpdateMuestraAsync(muestra);
            var muestraActualizada = await _context.Muestras.FirstOrDefaultAsync(m => m.IdMuestra == muestra.IdMuestra);

            // Assert
            Assert.IsNotNull(muestraActualizada);
            Assert.AreEqual(false, muestraActualizada.Estado);
        }

        // 3️⃣ Prueba: Eliminar muestra (borrado lógico)
        [TestMethod]
        public async Task EliminarMuestraAsync_DeberiaMarcarInactiva()
        {
            // Arrange
            var muestra = new Muestra
            {
                IdOrdenExamen = 1,
                IdTipoMuestra = 1,
                Estado = true,
                FechaRecepcion = System.DateTime.Now
            };
            await _repository.AddMuestraAsync(muestra);

            // Act
            muestra.Estado = false;
            await _repository.UpdateMuestraAsync(muestra);

            // Assert
            var muestraEliminada = await _context.Muestras.FirstOrDefaultAsync(m => m.IdMuestra == muestra.IdMuestra);
            Assert.IsNotNull(muestraEliminada);
            Assert.AreEqual(false, muestraEliminada.Estado);
        }

        // 4️⃣ Prueba: Obtener muestra por Id
        [TestMethod]
        public async Task ObtenerMuestraPorIdAsync_DeberiaRetornarMuestraCorrecta()
        {
            // Arrange
            var muestra = new Muestra
            {
                IdOrdenExamen = 1,
                IdTipoMuestra = 1,
                Estado = true,
                FechaRecepcion = System.DateTime.Now
            };
            await _repository.AddMuestraAsync(muestra);

            // Act
            var encontrada = await _service.ObtenerMuestraPorIdAsync(muestra.IdMuestra);

            // Assert
            Assert.IsNotNull(encontrada);
            Assert.AreEqual(muestra.IdMuestra, encontrada.IdMuestra);
        }

        // 5️⃣ Prueba: Obtener muestras activas
        [TestMethod]
        public async Task ObtenerMuestrasActivasAsync_DeberiaRetornarSoloActivas()
        {
            // Arrange
            var activa = new Muestra { IdOrdenExamen = 1, IdTipoMuestra = 1, Estado = true, FechaRecepcion = System.DateTime.Now };
            var inactiva = new Muestra { IdOrdenExamen = 1, IdTipoMuestra = 1, Estado = false, FechaRecepcion = System.DateTime.Now };

            await _repository.AddMuestraAsync(activa);
            await _repository.AddMuestraAsync(inactiva);

            // Act
            var activas = await _service.ObtenerMuestrasActivasAsync();

            // Assert
            Assert.IsTrue(activas.All(m => m.Estado == true));
        }
    }
}

