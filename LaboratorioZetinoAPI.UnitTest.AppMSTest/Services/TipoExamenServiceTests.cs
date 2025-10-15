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
    public class TipoExamenServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private TipoExamenRepository _repository = null!;
        private TipoExamenService _service = null!;

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
            _repository = new TipoExamenRepository(_context);
            _service = new TipoExamenService(_repository);
        }

        // 🔹 Limpieza después de cada prueba
        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Prueba: Agregar un nuevo tipo de examen
        [TestMethod]
        public async Task AgregarTipoExamenAsync_DeberiaAgregarEnBaseDeDatos()
        {
            // Arrange → Se prepara el objeto con datos de ejemplo
            var tipo = new TipoExamen
            {
                Nombre = "Hemograma Completo",
                Descripcion = "Análisis general de sangre",
                Precio = 15.00m
            };

            // Act → Se ejecuta el método del servicio
            var resultado = await _service.AgregarTipoExamenAsync(tipo);

            // Assert → Se verifican los resultados esperados
            var guardado = await _context.TiposExamen.FirstOrDefaultAsync(t => t.Nombre == "Hemograma Completo");
            Assert.IsNotNull(guardado);
            Assert.AreEqual("Tipo de examen agregado correctamente", resultado);
            Assert.AreEqual(true, guardado.Estado);
        }

        // 2️⃣ Prueba: Modificar tipo de examen existente
        [TestMethod]
        public async Task ModificarTipoExamenAsync_DeberiaActualizarDatos()
        {
            // Arrange
            var tipo = new TipoExamen
            {
                Nombre = "Examen de Orina",
                Descripcion = "Análisis básico",
                Precio = 10.00m,
                Estado = true
            };

            await _repository.AddTipoExamenAsync(tipo);

            // Se cambian los valores del objeto para probar la actualización
            tipo.Nombre = "Examen de Orina Completo";
            tipo.Precio = 12.50m;

            // Act
            var resultado = await _service.ModificarTipoExamenAsync(tipo);
            var actualizado = await _context.TiposExamen.FirstOrDefaultAsync(t => t.IdTipoExamen == tipo.IdTipoExamen);

            // Assert
            Assert.AreEqual("Tipo de examen modificado correctamente", resultado);
            Assert.AreEqual("Examen de Orina Completo", actualizado!.Nombre);
            Assert.AreEqual(12.50m, actualizado.Precio);
        }

        // 3️⃣ Prueba: Cancelar tipo de examen (borrado lógico → cambia el estado a inactivo)
        [TestMethod]
        public async Task CancelarTipoExamenAsync_DeberiaMarcarInactivo()
        {
            // Arrange → Se crea un tipo de examen activo
            var tipo = new TipoExamen
            {
                Nombre = "Examen de Glucosa",
                Descripcion = "Medición de niveles de azúcar",
                Precio = 5.00m,
                Estado = true
            };
            await _repository.AddTipoExamenAsync(tipo);

            // Act → Se cancela (equivalente a borrado lógico)
            var resultado = await _service.CancelarTipoExamenAsync(tipo.IdTipoExamen);
            var cancelado = await _context.TiposExamen.FirstOrDefaultAsync(t => t.IdTipoExamen == tipo.IdTipoExamen);

            // Assert → Se verifica que el registro siga existiendo pero esté inactivo
            Assert.AreEqual("Tipo de examen cancelado correctamente", resultado);
            Assert.IsNotNull(cancelado);
            Assert.AreEqual(false, cancelado!.Estado);
        }

        // 4️⃣ Prueba: Obtener tipo de examen por Id
        [TestMethod]
        public async Task ObtenerTipoExamenPorIdAsync_DeberiaRetornarCorrecto()
        {
            // Arrange
            var tipo = new TipoExamen
            {
                Nombre = "Perfil Lipídico",
                Descripcion = "Colesterol y triglicéridos",
                Precio = 20.00m,
                Estado = true
            };
            await _repository.AddTipoExamenAsync(tipo);

            // Act
            var encontrado = await _service.ObtenerTipoExamenPorIdAsync(tipo.IdTipoExamen);

            // Assert
            Assert.IsNotNull(encontrado);
            Assert.AreEqual(tipo.IdTipoExamen, encontrado!.IdTipoExamen);
        }

        // 5️⃣ Prueba: Obtener tipos de examen activos
        [TestMethod]
        public async Task ObtenerTiposExamenActivosAsync_DeberiaRetornarSoloActivos()
        {
            // Arrange → Se agregan dos tipos de examen (uno activo y otro inactivo)
            var activo = new TipoExamen { Nombre = "Prueba 1", Descripcion = "A", Precio = 5, Estado = true };
            var inactivo = new TipoExamen { Nombre = "Prueba 2", Descripcion = "B", Precio = 7, Estado = false };
            await _repository.AddTipoExamenAsync(activo);
            await _repository.AddTipoExamenAsync(inactivo);

            // Act
            var activos = await _service.ObtenerTiposExamenActivosAsync();

            // Assert → Solo deben aparecer los que tengan Estado = true
            Assert.IsTrue(activos.All(t => t.Estado == true));
        }

        // 6️⃣ Prueba: Eliminar tipo de examen (borrado físico → se elimina de la base)
        [TestMethod]
        public async Task EliminarTipoExamenAsync_DeberiaEliminarDeBaseDeDatos()
        {
            // Arrange → Se agrega un registro que luego será eliminado
            var tipo = new TipoExamen
            {
                Nombre = "Examen de Urea",
                Descripcion = "Prueba renal",
                Precio = 8.50m,
                Estado = true
            };
            await _repository.AddTipoExamenAsync(tipo);

            // Act → Se llama al método que elimina físicamente
            var resultado = await _service.EliminarTipoExamenAsync(tipo.IdTipoExamen);
            var eliminado = await _context.TiposExamen.FirstOrDefaultAsync(t => t.IdTipoExamen == tipo.IdTipoExamen);

            // Assert → Se espera que el registro ya no exista en la base
            Assert.AreEqual("Tipo de examen eliminado correctamente", resultado);
            Assert.IsNull(eliminado);
        }
    }
}
