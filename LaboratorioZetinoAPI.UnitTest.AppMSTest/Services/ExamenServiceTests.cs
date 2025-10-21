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
    public class ExamenServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private ExamenRepository _repository = null!;
        private ExamenService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // Ruta hacia el proyecto web donde está el appsettings.json
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
            _repository = new ExamenRepository(_context);
            _service = new ExamenService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Prueba: Agregar un nuevo examen
        [TestMethod]
        public async Task AgregarExamenAsync_DeberiaAgregarEnBaseDeDatos()
        {
            // Arrange
            var examen = new Examen
            {
                IdOrdenExamen = 1,
                IdTipoExamen = 1,
                Descripcion = "ExamenPrueba_" + System.Guid.NewGuid(),
                TiempoEstimado = 30,
                Estado = true
            };

            // Act
            var resultado = await _repository.AddExamenAsync(examen);

            // Assert
            var examenGuardado = await _context.Examenes.FirstOrDefaultAsync(e => e.IdExamen == resultado.IdExamen);
            Assert.IsNotNull(examenGuardado);
            Assert.AreEqual(true, examenGuardado.Estado);
        }

        // 2️⃣ Prueba: Modificar un examen existente
        [TestMethod]
        public async Task ModificarExamenAsync_DeberiaActualizarDescripcion()
        {
            // Arrange
            var examen = new Examen
            {
                IdOrdenExamen = 1,
                IdTipoExamen = 1,
                Descripcion = "ExamenModificar_" + System.Guid.NewGuid(),
                TiempoEstimado = 20,
                Estado = true
            };

            await _repository.AddExamenAsync(examen);
            var nuevaDescripcion = "ExamenActualizado_" + System.Guid.NewGuid();
            examen.Descripcion = nuevaDescripcion;

            // Act
            await _repository.UpdateExamenAsync(examen);
            var examenActualizado = await _context.Examenes.FirstOrDefaultAsync(e => e.IdExamen == examen.IdExamen);

            // Assert
            Assert.IsNotNull(examenActualizado);
            Assert.AreEqual(nuevaDescripcion, examenActualizado.Descripcion);
        }

        // 3️⃣ Prueba: Eliminar examen (borrado lógico)
        [TestMethod]
        public async Task EliminarExamenAsync_DeberiaMarcarInactivo()
        {
            // Arrange
            var examen = new Examen
            {
                IdOrdenExamen = 1,
                IdTipoExamen = 1,
                Descripcion = "ExamenEliminar_" + System.Guid.NewGuid(),
                TiempoEstimado = 25,
                Estado = true
            };
            await _repository.AddExamenAsync(examen);

            // Act
            examen.Estado = false;
            await _repository.UpdateExamenAsync(examen);

            // Assert
            var examenEliminado = await _context.Examenes.FirstOrDefaultAsync(e => e.IdExamen == examen.IdExamen);
            Assert.IsNotNull(examenEliminado);
            Assert.AreEqual(false, examenEliminado.Estado);
        }

        // 4️⃣ Prueba: Obtener examen por Id (solo activos)
        [TestMethod]
        public async Task ObtenerExamenPorIdAsync_DeberiaRetornarSoloActivos()
        {
            // Arrange
            var examenActivo = new Examen
            {
                IdOrdenExamen = 1,
                IdTipoExamen = 1,
                Descripcion = "ExamenActivo_" + System.Guid.NewGuid(),
                TiempoEstimado = 40,
                Estado = true
            };
            var examenInactivo = new Examen
            {
                IdOrdenExamen = 1,
                IdTipoExamen = 1,
                Descripcion = "ExamenInactivo_" + System.Guid.NewGuid(),
                TiempoEstimado = 40,
                Estado = false
            };

            await _repository.AddExamenAsync(examenActivo);
            await _repository.AddExamenAsync(examenInactivo);

            // Act
            var encontradoActivo = await _service.ObtenerExamenPorIdAsync(examenActivo.IdExamen);
            var encontradoInactivo = await _service.ObtenerExamenPorIdAsync(examenInactivo.IdExamen);

            // Assert
            Assert.IsNotNull(encontradoActivo, "El examen activo debe encontrarse");
            Assert.IsNull(encontradoInactivo, "El examen inactivo no debe retornarse");
        }
    }
}
