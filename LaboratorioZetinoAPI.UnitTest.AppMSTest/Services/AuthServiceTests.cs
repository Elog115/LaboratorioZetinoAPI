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
    public class AuthServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private UsuarioRepository _repository = null!;
        private AuthService _service = null!;

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
            _repository = new UsuarioRepository(_context);
            _service = new AuthService(_repository, configuration);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task RegisterAsync_DeberiaAgregarUsuario()
        {
            var usuario = new Usuario
            {
                Nombre = "UsuarioPrueba" + Guid.NewGuid(),
                Apellido = "Funcional",
                Email = "prueba" + Guid.NewGuid() + "@test.com",
                PasswordHash = "123456",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "12345678",
                IdRol = 1
            };

            var (ok, msg) = await _service.RegisterAsync(usuario.Nombre, usuario.Email, usuario.PasswordHash!, usuario.IdRol);

            Assert.IsTrue(ok);
            Assert.AreEqual("Usuario registrado", msg);

            var usuarioGuardado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            Assert.IsNotNull(usuarioGuardado);
        }

        [TestMethod]
        public async Task RegisterAsync_DeberiaRetornarErrorSiEmailDuplicado()
        {
            var email = "duplicado" + Guid.NewGuid() + "@test.com";

            var usuarioInicial = new Usuario
            {
                Nombre = "Original",
                Apellido = "Test",
                Email = email,
                PasswordHash = "123",
                IdRol = 1
            };
            await _service.RegisterAsync(usuarioInicial.Nombre, usuarioInicial.Email, usuarioInicial.PasswordHash!, usuarioInicial.IdRol);

            var usuarioDuplicado = new Usuario
            {
                Nombre = "Duplicado",
                Apellido = "Test",
                Email = email,
                PasswordHash = "456",
                IdRol = 1
            };

            var (ok, msg) = await _service.RegisterAsync(usuarioDuplicado.Nombre, usuarioDuplicado.Email, usuarioDuplicado.PasswordHash!, usuarioDuplicado.IdRol);

            Assert.IsFalse(ok);
            Assert.AreEqual("El email ya está registrado", msg);
        }

        [TestMethod]
        public async Task LoginAsync_DeberiaRetornarTokenSiCredencialesValidas()
        {
            var email = "login.valido" + Guid.NewGuid() + "@test.com";
            var clave = "clave123";
            var usuario = new Usuario
            {
                Nombre = "LoginValido",
                Apellido = "Test",
                Email = email,
                PasswordHash = clave,
                IdRol = 1
            };
            await _service.RegisterAsync(usuario.Nombre, usuario.Email, usuario.PasswordHash!, usuario.IdRol);

            var (ok, token) = await _service.LoginAsync(email, clave);

            Assert.IsTrue(ok);
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public async Task LoginAsync_DeberiaRetornarErrorSiUsuarioInactivo()
        {
            var email = "inactivo" + Guid.NewGuid() + "@test.com";
            var usuario = new Usuario
            {
                Nombre = "Inactivo",
                Apellido = "Test",
                Email = email,
                PasswordHash = "clave123",
                IdRol = 1,
                Estado = false
            };
            await _repository.AddUsuarioAsync(usuario);

            var (ok, msg) = await _service.LoginAsync(email, usuario.PasswordHash!);

            Assert.IsFalse(ok);
            Assert.AreEqual("Usuario no encontrado o inactivo", msg);
        }

        [TestMethod]
        public async Task LoginAsync_DeberiaRetornarErrorSiClaveIncorrecta()
        {
            var email = "incorrecta" + Guid.NewGuid() + "@test.com";
            var usuario = new Usuario
            {
                Nombre = "ClaveIncorrecta",
                Apellido = "Test",
                Email = email,
                PasswordHash = "correcta123",
                IdRol = 1
            };
            await _service.RegisterAsync(usuario.Nombre, usuario.Email, usuario.PasswordHash!, usuario.IdRol);

            var (ok, msg) = await _service.LoginAsync(email, "incorrectaXYZ");

            Assert.IsFalse(ok);
            Assert.AreEqual("Credenciales inválidas", msg);
        }
    }
}

