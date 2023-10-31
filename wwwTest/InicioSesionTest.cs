using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace wwwTest;

[TestClass]
public class InicioSesionTest
{ 

    private static IWebDriver driver;
    private StringBuilder verificationErrors;
    private static string baseURL;
    private bool acceptNextAlert = true;

    [ClassInitialize]
    public static void InitializeClass(TestContext testContext)
    {
        driver = new EdgeDriver();
        baseURL = "https://www.google.com/";
    }

    [ClassCleanup]
    public static void CleanupClass()
    {
        try
        {
            //driver.Quit();// quit does not close the window
            driver.Close();
            driver.Dispose();
        }
        catch (Exception)
        {
            // Ignore errors if unable to close the browser
        }
    }

    [TestInitialize]
    public void InitializeTest()
    {
        verificationErrors = new StringBuilder();
    }

    [TestCleanup]
    public void CleanupTest()
    {
        Assert.AreEqual("", verificationErrors.ToString());
    }

    [TestMethod]
    public void TheInicioSesionAdminTest()
    {
        driver.Navigate().GoToUrl("https://localhost:3000/");
        driver.FindElement(By.LinkText("Login")).Click();
        driver.FindElement(By.Id("zemail")).Click();
        driver.FindElement(By.Id("zemail")).Clear();
        driver.FindElement(By.Id("zemail")).SendKeys("NoExiste@gmail.com");
        driver.FindElement(By.Id("zcontrasenaHash")).Click();
        driver.FindElement(By.Id("zcontrasenaHash")).Clear();
        driver.FindElement(By.Id("zcontrasenaHash")).SendKeys("test1234");
        driver.FindElement(By.XPath("//input[@value='Iniciar sesión']")).Click();
        try
        {
            Assert.AreEqual("El correo electrónico no está registrado.", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Login'])[2]/following::li[1]")).Text);
        }
        catch (Exception e)
        {
            verificationErrors.Append(e.Message);
        }
        driver.FindElement(By.XPath("//body")).Click();
        driver.FindElement(By.Id("zemail")).Click();
        driver.FindElement(By.Id("zemail")).Clear();
        driver.FindElement(By.Id("zemail")).SendKeys("admin@gmail.com");
        driver.FindElement(By.Id("zcontrasenaHash")).Click();
        driver.FindElement(By.Id("zcontrasenaHash")).Clear();
        driver.FindElement(By.Id("zcontrasenaHash")).SendKeys("pongo mal la contraseña");
        driver.FindElement(By.XPath("//input[@value='Iniciar sesión']")).Click();
        try
        {
            Assert.AreEqual("La contraseña es incorrecta.", driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Login'])[2]/following::li[1]")).Text);
        }
        catch (Exception e)
        {
            verificationErrors.Append(e.Message);
        }
    }
    private bool IsElementPresent(By by)
    {
        try
        {
            driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    private bool IsAlertPresent()
    {
        try
        {
            driver.SwitchTo().Alert();
            return true;
        }
        catch (NoAlertPresentException)
        {
            return false;
        }
    }

    private string CloseAlertAndGetItsText()
    {
        try
        {
            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            if (acceptNextAlert)
            {
                alert.Accept();
            }
            else
            {
                alert.Dismiss();
            }
            return alertText;
        }
        finally
        {
            acceptNextAlert = true;
        }
    }
}

