using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Repositories;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SisLabZetino.Tests.Functional
{
    [TestClass]
    public class TipoMuestraServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private TipoMuestraRepository _repository = null!;
        private TipoMuestraService _service = null!;

        // 🔹 Configuración inicial antes de cada prueba
        [TestInitialize]
        public void Setup()
        {
            // Ruta hacia el proyecto web donde está el archivo appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\LabZetino.Web");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Obtiene la cadena de conexión a la base de datos
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            _context = new AppDBContext(options);
            _repository = new TipoMuestraRepository(_context);
            _service = new TipoMuestraService(_repository);
        }

        // 🔹 Limpieza después de cada prueba
        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Prueba: Agregar un nuevo tipo de muestra
        [TestMethod]
        public async Task AgregarTipoMuestraAsync_DeberiaAgregarEnBaseDeDatos()
        {
            // Arrange → Se prepara el objeto con datos de ejemplo
            var tipo = new TipoMuestra
            {
                Nombre = "Sangre",
                Descripcion = "Análisis de sangre"
            };

            // Act → Se ejecuta el método del servicio
            var resultado = await _service.AgregarTipoMuestraAsync(tipo);

            // Assert → Se verifican los resultados esperados
            var guardado = await _context.TiposMuestra.FirstOrDefaultAsync(t => t.Nombre == "Sangre");
            Assert.IsNotNull(guardado);
            Assert.AreEqual("Tipo de muestra agregado correctamente", resultado);
            Assert.AreEqual(true, guardado.Estado);
        }

        // 2️⃣ Prueba: Modificar tipo de muestra existente
        [TestMethod]
        public async Task ModificarTipoMuestraAsync_DeberiaActualizarDatos()
        {
            // Arrange → Se crea un registro en la base
            var tipo = new TipoMuestra
            {
                Nombre = "Orina",
                Descripcion = "Análisis básico",
                Estado = true
            };

            await _repository.AddTipoMuestraAsync(tipo);

            // Se modifican los datos
            tipo.Nombre = "Orina Completa";
            tipo.Descripcion = "Muestra de orina actualizada";

            // Act → Se llama al método de modificación
            var resultado = await _service.ModificarTipoMuestraAsync(tipo);
            var actualizado = await _context.TiposMuestra.FirstOrDefaultAsync(t => t.IdTipoMuestra == tipo.IdTipoMuestra);

            // Assert → Se verifica la actualización
            Assert.AreEqual("Tipo de muestra modificado correctamente", resultado);
            Assert.AreEqual("Orina Completa", actualizado!.Nombre);
        }

        // 3️⃣ Prueba: Cancelar tipo de muestra (borrado lógico → cambia el estado a inactivo)
        [TestMethod]
        public async Task CancelarTipoMuestraAsync_DeberiaMarcarInactiva()
        {
            // Arrange → Se crea un tipo de muestra activo
            var tipo = new TipoMuestra
            {
                Nombre = "Saliva",
                Descripcion = "Análisis de saliva",
                Estado = true
            };
            await _repository.AddTipoMuestraAsync(tipo);

            // Act → Se cancela (borrado lógico)
            var resultado = await _service.CancelarTipoMuestraAsync(tipo.IdTipoMuestra);
            var cancelado = await _context.TiposMuestra.FirstOrDefaultAsync(t => t.IdTipoMuestra == tipo.IdTipoMuestra);

            // Assert → Se verifica que el registro sigue existiendo pero está inactivo
            Assert.AreEqual("Tipo de muestra cancelado correctamente", resultado);
            Assert.IsNotNull(cancelado);
            Assert.AreEqual(false, cancelado!.Estado);
        }

        // 4️⃣ Prueba: Obtener tipo de muestra por Id
        [TestMethod]
        public async Task ObtenerTipoMuestraPorIdAsync_DeberiaRetornarCorrecto()
        {
            // Arrange
            var tipo = new TipoMuestra
            {
                Nombre = "Tejido",
                Descripcion = "Muestra de tejido",
                Estado = true
            };
            await _repository.AddTipoMuestraAsync(tipo);

            // Act
            var encontrado = await _service.ObtenerTipoMuestraPorIdAsync(tipo.IdTipoMuestra);

            // Assert
            Assert.IsNotNull(encontrado);
            Assert.AreEqual(tipo.IdTipoMuestra, encontrado!.IdTipoMuestra);
        }

        // 5️⃣ Prueba: Obtener tipos de muestra activos
        [TestMethod]
        public async Task ObtenerTiposMuestraActivosAsync_DeberiaRetornarSoloActivos()
        {
            // Arrange → Se agregan dos tipos de muestra (uno activo y otro inactivo)
            var activo = new TipoMuestra { Nombre = "Plasma", Descripcion = "Muestra A", Estado = true };
            var inactivo = new TipoMuestra { Nombre = "Suero", Descripcion = "Muestra B", Estado = false };
            await _repository.AddTipoMuestraAsync(activo);
            await _repository.AddTipoMuestraAsync(inactivo);

            // Act
            var activos = await _service.ObtenerTiposMuestraActivosAsync();

            // Assert → Solo deben aparecer los que tengan Estado = true
            Assert.IsTrue(activos.All(t => t.Estado == true));
        }

        // 6️⃣ Prueba: Eliminar tipo de muestra (borrado físico → se elimina de la base)
        [TestMethod]
        public async Task EliminarTipoMuestraAsync_DeberiaEliminarDeBaseDeDatos()
        {
            // Arrange → Se agrega un registro que luego será eliminado
            var tipo = new TipoMuestra
            {
                Nombre = "Tejido Óseo",
                Descripcion = "Muestra sólida",
                Estado = true
            };
            await _repository.AddTipoMuestraAsync(tipo);

            // Act → Se llama al método que elimina físicamente
            var resultado = await _service.EliminarTipoMuestraAsync(tipo.IdTipoMuestra);
            var eliminado = await _context.TiposMuestra.FirstOrDefaultAsync(t => t.IdTipoMuestra == tipo.IdTipoMuestra);

            // Assert → Se espera que el registro ya no exista
            Assert.AreEqual("Tipo de muestra eliminado correctamente", resultado);
            Assert.IsNull(eliminado);
        }
    }
}
