namespace GenTreeLibTest;
using UBUGenTreeMVC.Models;

[TestClass]
public class UtilsTest1
{
    [TestMethod]
    public void TestEncriptar()
    {
        string contrasena1 = "test1234";
        string contrasena2 = "test1234";
        string contrasenaDiferente = "Test1234";

        string hash1 = Utils.Encriptar(contrasena1);
        string hash2 = Utils.Encriptar(contrasena2);
        string hash3 = Utils.Encriptar(contrasenaDiferente);

        Assert.AreEqual(hash1, hash2);
        Assert.AreNotEqual(hash1, hash3);
    }

    [TestMethod]
    public void TestNivelComplejidad()
    {
        string[] contrasenas = new string[] { "contraseña1", "Contraseña2", "CONTRASEÑA3", "Contraseña123!", "contraseña", "CONTRASEÑA", "12345678", "!@#$%^&*" };

        foreach (string contrasena in contrasenas)
        {

            int resultado = Utils.NivelComplejidad(contrasena);

            if (contrasena.Length < 8)
            {
                Assert.AreEqual(0, resultado);
            }
            else
            {
                int esperado = 0;
                esperado += contrasena.Any(char.IsLower) ? 1 : 0;
                esperado += contrasena.Any(char.IsUpper) ? 1 : 0;
                esperado += contrasena.Any(char.IsDigit) ? 1 : 0;
                esperado += contrasena.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)) ? 1 : 0;
                Assert.AreEqual(esperado, resultado);
            }
        }
    }

    [DataTestMethod]
    [DataRow("test@example.com", true)]
    [DataRow("test.example.com", false)]
    [DataRow("test@example", false)]
    [DataRow("test@.com", false)]
    [DataRow("test@", false)]
    [DataRow("@example.com", false)]
    public void TestEsEMail(string email, bool esperado)
    {
        bool resultado = Utils.EsEMail(email);
        Assert.AreEqual(esperado, resultado);
    }
}
