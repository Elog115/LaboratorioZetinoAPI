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
    public class AuthServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private UsuarioRepository _usuarioRepository = null!;
        private AuthService _authService = null!;
        private IConfiguration _configuration = null!;

        [TestInitialize]
        public void Setup()
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\LabZetino.Web");

            _configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            _context = new AppDBContext(options);
            _usuarioRepository = new UsuarioRepository(_context);
            _authService = new AuthService(_usuarioRepository, _configuration);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Registrar usuario
        [TestMethod]
        public async Task RegisterAsync_DeberiaAgregarUsuario()
        {
            var (ok, msg) = await _authService.RegisterAsync("TestUser", "testuser@test.com", "Password123!", 1);
            Assert.IsTrue(ok);
            Assert.AreEqual("Usuario registrado", msg);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "testuser@test.com");
            Assert.IsNotNull(usuario);
            Assert.AreEqual("TestUser", usuario.Nombre);
        }

        // 2️⃣ Login exitoso
        [TestMethod]
        public async Task LoginAsync_DeberiaRetornarToken()
        {
            await _authService.RegisterAsync("LoginUser", "login@test.com", "Password123!", 1);

            var (ok, token) = await _authService.LoginAsync("login@test.com", "Password123!");
            Assert.IsTrue(ok);
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        // 3️⃣ Login fallido
        [TestMethod]
        public async Task LoginAsync_ConPasswordIncorrecto_DeberiaFallar()
        {
            await _authService.RegisterAsync("FailUser", "fail@test.com", "Password123!", 1);

            var (ok, msg) = await _authService.LoginAsync("fail@test.com", "WrongPassword");
            Assert.IsFalse(ok);
            Assert.AreEqual("Credenciales inválidas", msg);
        }

        // 4️⃣ Obtener usuario por Id
        [TestMethod]
        public async Task ObtenerUsuarioPorIdAsync_DeberiaRetornarUsuarioActivo()
        {
            var (ok, _) = await _authService.RegisterAsync("GetUser", "getuser@test.com", "Password123!", 1);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "getuser@test.com");

            var encontrado = await _authService.ObtenerUsuarioPorIdAsync(usuario!.IdUsuario);
            Assert.IsNotNull(encontrado);
            Assert.AreEqual("getuser@test.com", encontrado.Email);
        }

        // 5️⃣ Modificar usuario
        [TestMethod]
        public async Task ModificarUsuarioAsync_DeberiaActualizarDatos()
        {
            await _authService.RegisterAsync("ModUser", "moduser@test.com", "Password123!", 1);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "moduser@test.com");

            usuario!.Nombre = "UsuarioModificado";
            usuario.PasswordHash = "NewPassword123!";

            var resultado = await _authService.ModificarUsuarioAsync(usuario);
            Assert.AreEqual("Usuario modificado correctamente", resultado);

            var modificado = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
            Assert.AreEqual("UsuarioModificado", modificado!.Nombre);
        }

        // 6️⃣ Eliminar usuario
        [TestMethod]
        public async Task EliminarUsuarioAsync_DeberiaMarcarInactivo()
        {
            await _authService.RegisterAsync("DelUser", "deluser@test.com", "Password123!", 1);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "deluser@test.com");

            var resultado = await _authService.EliminarUsuarioAsync(usuario!.IdUsuario);
            Assert.AreEqual("Usuario eliminado correctamente", resultado);

            var eliminado = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
            Assert.IsFalse(eliminado!.Estado);
        }

        // 7️⃣ Obtener usuarios activos
        [TestMethod]
        public async Task ObtenerUsuariosActivosAsync_DeberiaRetornarSoloActivos()
        {
            await _authService.RegisterAsync("ActUser1", "act1@test.com", "Password123!", 1);
            await _authService.RegisterAsync("ActUser2", "act2@test.com", "Password123!", 1);

            // Eliminar uno
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "act2@test.com");
            await _authService.EliminarUsuarioAsync(usuario!.IdUsuario);

            var activos = await _authService.ObtenerUsuariosActivosAsync();
            Assert.IsTrue(activos.All(u => u.Estado == true));
            Assert.IsTrue(activos.Any(u => u.Email == "act1@test.com"));
            Assert.IsFalse(activos.Any(u => u.Email == "act2@test.com"));
        }
    }
}
