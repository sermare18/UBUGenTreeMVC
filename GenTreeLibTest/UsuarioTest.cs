namespace GenTreeLibTest;

using Newtonsoft.Json;
using UBUGenTreeMVC.Models;

[TestClass]

public class UsuarioTest
{

    [TestMethod]
    public void TestAncestros_ReturnsNull()
    {
        // Arrange
        Usuario usuario = new Usuario();
        usuario._ancestrosJSON = null;

        // Act
        var result = usuario._ancestros;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TestAncestros_ReturnsList()
    {
        // Arrange
        Usuario usuario = new Usuario();
        List<Persona> personas = new List<Persona>()
            {
                new Persona { Nombre = "Persona1" },
                new Persona { Nombre = "Persona2" }
            };
        usuario._ancestrosJSON = Newtonsoft.Json.JsonConvert.SerializeObject(personas);

        // Act
        var result = usuario._ancestros;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void TestCrearAdministrador()
    {
        // Arrange
        string nombre = "Alberto Lanchares";
        string email = "alberto.lachares@gmail.com";
        string contraseñaHash = "passwd123";

        // Act
        Usuario usuario = Usuario.CrearAdministrador(nombre, email, contraseñaHash);

        // Assert
        Assert.AreEqual(Roles.Administrador, usuario._rol);
        Assert.AreEqual(EstadoCuenta.Activa, usuario._estado);
    }

    [TestMethod]
    public void TestCrearGestor()
    {
        // Arrange
        string nombre = "Sergio Mertin";
        string email = "sergio.martin@gmail.com";
        string contraseñaHash = "contra123";

        // Act
        Usuario usuario = Usuario.CrearGestor(nombre, email, contraseñaHash);

        // Assert
        Assert.AreEqual(Roles.Gestor, usuario._rol);
        Assert.AreEqual(EstadoCuenta.Activa, usuario._estado);
    }

    [TestMethod]
    public void CrearUsuario()
    {
        // Arrange
        string nombre = "Leire Garcia";
        string email = "leire.garcia@gmail.com";
        string contraseñaHash = "passwd987";

        // Act
        Usuario usuario = Usuario.CrearUsuario(nombre, email, contraseñaHash);

        // Assert
        Assert.AreEqual(Roles.Usuario, usuario._rol);
        Assert.AreEqual(EstadoCuenta.Solicitada, usuario._estado);
    }

    [TestMethod]
    public void TestToString()
    {
        // Arrange
        Usuario usuario = new Usuario
        {
            _id = 1,
            _nombre = "John Doe",
            _email = "john.doe@example.com",
            _rol = Roles.Administrador,
            _estado = EstadoCuenta.Activa,
            _ultimoIngreso = DateTime.Now,
            _intentosFallidos = 3
        };

        string expectedString = $"ID: {usuario._id}, Nombre: {usuario._nombre}, Email: {usuario._email}, Rol: {usuario._rol}, Estado de la cuenta: {usuario._estado}, Último ingreso: {usuario._ultimoIngreso}, Intentos fallidos: {usuario._intentosFallidos}";

        // Act
        string result = usuario.ToString();

        // Assert
        Assert.AreEqual(expectedString, result);
    }

    [TestMethod]
    public void TestGetUsuario()
    {
        // Arrange
        string nombre = "John Doe";
        string email = "john.doe@example.com";
        string contraseñaHash = "contra987";

        // Act
        Usuario usuario = Usuario.CrearAdministrador(nombre, email, contraseñaHash);

        // Assert
        Assert.AreEqual(usuario, usuario.GetUsuario());
    }

    [TestMethod]
    public void TestGetAncestros()
    {
        // Arrange
        var usuario = new Usuario();
        var expected = new List<Persona>();

        // Act
        var actual = usuario.GetAncestros();

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestSetAncestros()
    {
        // Arrange
        Usuario usuario = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona();
        Persona persona2 = new Persona();
        Persona persona3 = new Persona();

        // Act
        usuario.SetAncestros(persona1, persona2, persona3);

        // Assert
        Assert.AreEqual(3, usuario._ancestros.Count);
    }

    [TestMethod]
    public void TestGetIdsAncestros_PersonaValida()
    {
        // Arrange
        Usuario usuario = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona { _id = 1, A1_id = 2, A2_id = 3 };
        Persona persona2 = new Persona { _id = 2, A1_id = null, A2_id = null };
        Persona persona3 = new Persona { _id = 3, A1_id = null, A2_id = null };
        usuario.SetAncestros(persona1, persona2, persona3);

        // Act
        (int? a1Id, int? a2Id) = usuario.GetIdsAncestros(1);

        // Assert
        Assert.AreEqual(2, a1Id);
        Assert.AreEqual(3, a2Id);
    }

    [TestMethod]
    public void TestGetIdsAncestros_PersonaNoValida()
    {
        // Arrange
        Usuario usuario = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");

        // Act
        (int? a1Id, int? a2Id) = usuario.GetIdsAncestros(10);

        // Assert
        Assert.IsNull(a1Id);
        Assert.IsNull(a2Id);
    }

    [TestMethod]
    public void TestGetIdsAncestros_PersonaConAncestrosNull()
    {
        // Arrange
        Usuario usuario = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona { _id = 1 };
        usuario.SetPrimeraPersona(persona1);

        // Act
        (int? a1Id, int? a2Id) = usuario.GetIdsAncestros(1);

        // Assert
        Assert.IsNull(a1Id);
        Assert.IsNull(a2Id);
    }



    [TestMethod]
    public void TestAnadirAncestrosValidos()
    {
        // Arrange
        Usuario usuarioValido = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona { Nombre = "Persona1", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now };
        Persona padre = new Persona { Nombre = "Padre", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now.AddYears(-30) };
        Persona madre = new Persona { Nombre = "Madre", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now.AddYears(-30) };

        usuarioValido.SetPrimeraPersona(persona1);

        // Caso 1: Usuario especificado es válido
        // Act
        usuarioValido.AnadirAncestros(persona1, padre, madre);

        // Assert
        Assert.IsNotNull(usuarioValido._ancestros);
        Assert.AreEqual(3, usuarioValido._ancestros.Count);

    }

    [TestMethod]
    public void TestAnadirAncestrosNoValidos()
    {
        // Arrange
        Usuario usuarioValido = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona { Nombre = "Persona1", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now.AddYears(-30) };
        Persona padre = new Persona { Nombre = "Padre", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now };
        Persona madre = new Persona { Nombre = "Madre", Apellido1 = "Apellido1", Apellido2 = "Apellido2", Localidad = "Localidad", FechaNac = DateTime.Now };

        usuarioValido.SetPrimeraPersona(persona1);

        // Caso 1: Usuario especificado es válido
        // Act
        usuarioValido.AnadirAncestros(persona1, padre, madre);

        // Assert
        Assert.IsNotNull(usuarioValido._ancestros);
        Assert.AreEqual(1, usuarioValido._ancestros.Count);

    }


    [TestMethod]
    public void TestSetPrimeraPersona()
    {
        // Arrange
        Usuario usuario = Usuario.CrearUsuario("TestUser", "test@gmail.com", "passwd456");
        Persona persona1 = new Persona { Nombre = "Persona1" };

        // Act
        usuario.SetPrimeraPersona(persona1);

        // Assert
        Assert.IsNotNull(usuario._ancestros);
        Assert.AreEqual(1, usuario._ancestros.Count);
        Assert.AreEqual("Persona1", usuario._ancestros[0].Nombre);
    }


    [TestMethod]
    public void TestValidarCuenta()
    {
        // Arrange
        string nombre_admin = "John Doe";
        string email_admin = "john.doe@gmail.com";
        string contraseñaHash_admin = "passwd123";

        string nombre_usuario = "Pepe";
        string email_usuario = "pepe@gmail.com";
        string contraseñaHash_usuario = "test1234";

        Usuario administrador = Usuario.CrearAdministrador(nombre_admin, email_admin, contraseñaHash_admin);
        Usuario usuarioNormal = Usuario.CrearUsuario(nombre_usuario, email_usuario, contraseñaHash_usuario);

        // Act
        administrador.ValidarCuenta(usuarioNormal);

        // Assert
        Assert.AreEqual(EstadoCuenta.Activa, usuarioNormal._estado);
    }


    [TestMethod]
    public void TestBloquearCuenta()
    {
        // Arrange
        string nombre_admin = "John Doe";
        string email_admin = "john.doe@gmail.com";
        string contraseñaHash_admin = "passwd123";

        string nombre_usuario = "Pepe";
        string email_usuario = "pepe@gmail.com";
        string contraseñaHash_usuario = "test1234";

        Usuario administrador = Usuario.CrearAdministrador(nombre_admin, email_admin, contraseñaHash_admin);
        Usuario usuarioNormal = Usuario.CrearUsuario(nombre_usuario, email_usuario, contraseñaHash_usuario);

        // Act
        administrador.BloquearCuenta(usuarioNormal);

        // Assert
        Assert.AreEqual(EstadoCuenta.Bloqueada, usuarioNormal._estado);
    }

}
