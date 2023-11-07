namespace GenTreeLibTest;

using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using UBUGenTreeMVC.Models;

[TestClass]
public class PersonaTests
{


    [TestMethod]
    public void TestConstructor()
    {
        // Arrange
        int expectedId = 0;

        // Act
        Persona persona = new Persona();

        // Assert
        Assert.AreEqual(expectedId, persona._id);
    }

    [TestMethod]
    public void TestToString()
    {
        // Crear un objeto con valores conocidos
        var persona = new Persona
        {
            _id = 1,
            Nombre = "Juan",
            Apellido1 = "Pérez",
            Apellido2 = "García",
            Localidad = "Burgos",
            FechaNac = new DateTime(1990, 1, 1),
            FechaDef = null,
            A1_id = 2,
            A2_id = 3
        };

        // Llamar a la función ToString() en el objeto
        var result = persona.ToString();

        // Comparar la cadena generada con la cadena esperada
        var expected = "ID: 1, Nombre: Juan, Primer Apellido: Pérez, Segundo Apellido: García, Localidad: Burgos, Fecha de Nacimiento: 1990-01-01, Fecha de Defunción: N/A, id_padre: 2, id_madre: 3";
        Assert.AreEqual(expected, result);
    }

}
