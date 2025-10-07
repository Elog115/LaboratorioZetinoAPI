using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Entities;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Repositories;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace SisLabZetino.Tests.Functional
{
    [TestClass]
    public class RolServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private RolRepository _repository = null!;
        private RolService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // Ruta base hacia el proyecto web donde está appsettings.json
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
            _repository = new RolRepository(_context);
            _service = new RolService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Prueba: Agregar un nuevo rol
        [TestMethod]
        public async Task AgregarRolAsync_AgregarRolEnBaseDeDatos()
        {
            // Arrange
            var rol = new Rol { Nombre = "RolPrueba_" + System.Guid.NewGuid(), Estado = true };

            // Act
            var resultado = await _repository.AddRolAsync(rol);

            // Assert
            var rolGuardado = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == resultado.IdRol);
            Assert.IsNotNull(rolGuardado);
            Assert.AreEqual(true, rolGuardado.Estado);
        }

        // 2️⃣ Prueba: Modificar un rol existente
        [TestMethod]
        public async Task ModificarRolAsync_ActualizarNombreDelRol()
        {
            // Arrange
            var rol = new Rol { Nombre = "RolModificar_" + System.Guid.NewGuid(), Estado = true };
            await _repository.AddRolAsync(rol);

            var nuevoNombre = "RolActualizado_" + System.Guid.NewGuid();
            rol.Nombre = nuevoNombre;

            // Act
            await _repository.UpdateRolAsync(rol);
            var rolActualizado = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == rol.IdRol);

            // Assert
            Assert.IsNotNull(rolActualizado);
            Assert.AreEqual(nuevoNombre, rolActualizado.Nombre);
        }

        // 3️⃣ Prueba: Eliminar rol (borrado lógico)

        [TestMethod]
        public async Task EliminarRolAsync_DesactivarElRol()
        {
            // Arrange
            var rol = new Rol { Nombre = "RolEliminar_" + System.Guid.NewGuid(), Estado = true };
            await _repository.AddRolAsync(rol);

            // Act
            // Simular borrado lógico (asumiendo que el método cambia Estado = false)
            rol.Estado = false;
            await _repository.UpdateRolAsync(rol);

            // Assert
            var rolEliminado = await _context.Roles.FirstOrDefaultAsync(r => r.IdRol == rol.IdRol);
            Assert.IsNotNull(rolEliminado);
            Assert.AreEqual(false, rolEliminado.Estado);
        }

        // 4️⃣ Prueba: Obtener rol por Id (solo activos)
     
        [TestMethod]
        public async Task ObtenerRolPorIdAsync_RetornarSoloActivos()
        {
            // Arrange
            var rolActivo = new Rol { Nombre = "RolActivo_" + System.Guid.NewGuid(), Estado = true };
            var rolInactivo = new Rol { Nombre = "RolInactivo_" + System.Guid.NewGuid(), Estado = false };

            await _repository.AddRolAsync(rolActivo);
            await _repository.AddRolAsync(rolInactivo);

            // Act
            var encontradoActivo = await _service.ObtenerRolPorIdAsync(rolActivo.IdRol);
            var encontradoInactivo = await _service.ObtenerRolPorIdAsync(rolInactivo.IdRol);

            // Assert
            Assert.IsNotNull(encontradoActivo, "El rol activo debe encontrarse");
            Assert.IsNull(encontradoInactivo, "El rol inactivo no debe retornarse");
        }
    }
}
