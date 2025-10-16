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
    public class UsuarioServiceFunctionalTests
    {
        private AppDBContext _context = null!;
        private UsuarioRepository _repository = null!;
        private UsuarioService _service = null!;

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
            _service = new UsuarioService(_repository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // 1️⃣ Agregar un nuevo usuario
        [TestMethod]
        public async Task AgregarUsuarioAsync_DeberiaAgregarUsuarioEnBaseDeDatos()
        {
            var usuario = new Usuario
            {
                Nombre = "UsuarioPrueba" + Guid.NewGuid(),
                Apellido = "Funcional",
                Correo = "prueba" + Guid.NewGuid() + "@test.com",
                Clave = "123456",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "12345678",
                IdRol = 1,
                Estado = true
            };

            var resultado = await _service.AgregarUsuarioAsync(usuario);

            Assert.AreEqual("Usuario agregado correctamente", resultado);
            var usuarioGuardado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == usuario.Correo);
            Assert.IsNotNull(usuarioGuardado);
        }

        // 2️⃣ No debería agregar un usuario con un nombre duplicado
        [TestMethod]
        public async Task AgregarUsuarioAsync_DeberiaRetornarErrorSiNombreExiste()
        {
            var nombreExistente = "NombreDuplicado" + Guid.NewGuid();
            var usuarioInicial = new Usuario
            {
                Nombre = nombreExistente,
                Apellido = "Original",
                Correo = "original" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "11112222",
                IdRol = 1,
                Estado = true
            };
            await _service.AgregarUsuarioAsync(usuarioInicial);

            var usuarioDuplicado = new Usuario
            {
                Nombre = nombreExistente,
                Apellido = "Duplicado",
                Correo = "duplicado" + Guid.NewGuid() + "@test.com",
                Clave = "456",
                FechaNacimiento = new DateTime(1995, 5, 5),
                Telefono = "33334444",
                IdRol = 1,
                Estado = true
            };

            var resultado = await _service.AgregarUsuarioAsync(usuarioDuplicado);

            Assert.AreEqual("Error: Ya existe un usuario con el mismo nombre", resultado);
        }

        // 3️⃣ Modificar un usuario existente
        [TestMethod]
        public async Task ModificarUsuarioAsync_DeberiaActualizarElApellido()
        {
            var usuario = new Usuario
            {
                Nombre = "UsuarioModificar" + Guid.NewGuid(),
                Apellido = "Original",
                Correo = "modificar" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1985, 1, 1),
                Telefono = "55556666",
                IdRol = 1,
                Estado = true
            };
            await _repository.AddUsuarioAsync(usuario);

            var nuevoApellido = "Modificado";
            usuario.Apellido = nuevoApellido;

            var resultado = await _service.ModificarUsuarioAsync(usuario);

            Assert.AreEqual("Usuario modificado correctamente", resultado);
            var usuarioActualizado = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
            Assert.IsNotNull(usuarioActualizado);
            Assert.AreEqual(nuevoApellido, usuarioActualizado.Apellido);
        }

        // 4️⃣ Validar login con credenciales correctas y usuario activo
        [TestMethod]
        public async Task ValidateUsuarioAsync_DeberiaRetornarUsuarioSiEsValidoYActivo()
        {
            var correo = "login.valido" + Guid.NewGuid() + "@test.com";
            var clave = "claveSecreta123";
            var usuario = new Usuario
            {
                Nombre = "LoginValido",
                Apellido = "Test",
                Correo = correo,
                Clave = clave,
                FechaNacimiento = new DateTime(2000, 1, 1),
                Telefono = "77778888",
                IdRol = 1,
                Estado = true
            };
            await _repository.AddUsuarioAsync(usuario);

            var resultadoLogin = await _service.ValidateUsuarioAsync(correo, clave);

            Assert.IsNotNull(resultadoLogin);
            Assert.AreEqual(correo, resultadoLogin.Correo);
        }

        // 5️⃣ Validar login con credenciales correctas pero usuario inactivo
        [TestMethod]
        public async Task ValidateUsuarioAsync_DeberiaRetornarNuloSiUsuarioEstaInactivo()
        {
            var correo = "login.inactivo" + Guid.NewGuid() + "@test.com";
            var clave = "claveInactiva";
            var usuario = new Usuario
            {
                Nombre = "LoginInactivo",
                Apellido = "Test",
                Correo = correo,
                Clave = clave,
                FechaNacimiento = new DateTime(1999, 1, 1),
                Telefono = "99990000",
                IdRol = 1,
                Estado = false
            };
            await _repository.AddUsuarioAsync(usuario);

            var resultadoLogin = await _service.ValidateUsuarioAsync(correo, clave);

            Assert.IsNull(resultadoLogin);
        }

        // 6️⃣ Validar login con clave incorrecta
        [TestMethod]
        public async Task ValidateUsuarioAsync_DeberiaRetornarNuloConClaveIncorrecta()
        {
            var correo = "login.clave.incorrecta" + Guid.NewGuid() + "@test.com";
            var claveCorrecta = "claveCorrecta123";
            var claveIncorrecta = "claveIncorrectaXYZ";
            var usuario = new Usuario
            {
                Nombre = "LoginClaveIncorrecta",
                Apellido = "Test",
                Correo = correo,
                Clave = claveCorrecta,
                FechaNacimiento = new DateTime(1998, 1, 1),
                Telefono = "11223344",
                IdRol = 1,
                Estado = true
            };
            await _repository.AddUsuarioAsync(usuario);

            var resultadoLogin = await _service.ValidateUsuarioAsync(correo, claveIncorrecta);

            Assert.IsNull(resultadoLogin);
        }

        // 7️⃣ Eliminar un usuario (borrado físico)
        [TestMethod]
        public async Task EliminarUsuarioAsync_DeberiaRemoverloDeLaBD()
        {
            var usuario = new Usuario
            {
                Nombre = "UsuarioEliminar" + Guid.NewGuid(),
                Apellido = "Test",
                Correo = "eliminar" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1991, 1, 1),
                Telefono = "12123434",
                IdRol = 1,
                Estado = true
            };
            await _repository.AddUsuarioAsync(usuario);

            var resultado = await _service.EliminarUsuarioAsync(usuario.IdUsuario);

            Assert.AreEqual("Usuario eliminado correctamente", resultado);
            var usuarioEliminado = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
            Assert.IsNull(usuarioEliminado);
        }

        // 8️⃣ Eliminar un usuario que no existe
        [TestMethod]
        public async Task EliminarUsuarioAsync_DeberiaRetornarErrorSiNoExiste()
        {
            var idInexistente = -9999;
            var resultado = await _service.EliminarUsuarioAsync(idInexistente);
            Assert.AreEqual("Error: Usuario no encontrado", resultado);
        }

        // 9️⃣ Obtener solo usuarios activos
        [TestMethod]
        public async Task ObtenerUsuariosActivosAsync_DeberiaRetornarSoloUsuariosConEstadoTrue()
        {
            var usuarioActivo = new Usuario
            {
                Nombre = "Activo" + Guid.NewGuid(),
                Apellido = "U",
                Correo = "activo" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1992, 1, 1),
                Telefono = "56567878",
                IdRol = 1,
                Estado = true
            };
            var usuarioInactivo = new Usuario
            {
                Nombre = "Inactivo" + Guid.NewGuid(),
                Apellido = "U",
                Correo = "inactivo" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1993, 1, 1),
                Telefono = "98987676",
                IdRol = 1,
                Estado = false
            };
            await _repository.AddUsuarioAsync(usuarioActivo);
            await _repository.AddUsuarioAsync(usuarioInactivo);

            var resultado = await _service.ObtenerUsuariosActivosAsync();

            Assert.IsTrue(resultado.Any(u => u.IdUsuario == usuarioActivo.IdUsuario));
            Assert.IsFalse(resultado.Any(u => u.IdUsuario == usuarioInactivo.IdUsuario));
        }

        // 10️⃣ Obtener usuario por ID debe retornar nulo si el usuario está inactivo
        [TestMethod]
        public async Task ObtenerUsuarioPorIdAsync_DeberiaRetornarNuloParaUsuarioInactivo()
        {
            var usuarioInactivo = new Usuario
            {
                Nombre = "BuscarInactivo" + Guid.NewGuid(),
                Apellido = "Test",
                Correo = "buscar.inactivo" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1994, 1, 1),
                Telefono = "10293847",
                IdRol = 1,
                Estado = false
            };
            await _repository.AddUsuarioAsync(usuarioInactivo);

            var resultado = await _service.ObtenerUsuarioPorIdAsync(usuarioInactivo.IdUsuario);

            Assert.IsNull(resultado);
        }

        // 11️⃣ Obtener usuario por ID debe retornar el usuario si está activo
        [TestMethod]
        public async Task ObtenerUsuarioPorIdAsync_DeberiaRetornarUsuarioSiEstaActivo()
        {
            var usuarioActivo = new Usuario
            {
                Nombre = "BuscarActivo" + Guid.NewGuid(),
                Apellido = "Test",
                Correo = "buscar.activo" + Guid.NewGuid() + "@test.com",
                Clave = "123",
                FechaNacimiento = new DateTime(1994, 1, 1),
                Telefono = "10293848",
                IdRol = 1,
                Estado = true
            };
            await _repository.AddUsuarioAsync(usuarioActivo);

            var resultado = await _service.ObtenerUsuarioPorIdAsync(usuarioActivo.IdUsuario);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(usuarioActivo.IdUsuario, resultado.IdUsuario);
        }
    }
}

