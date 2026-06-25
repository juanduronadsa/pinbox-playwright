using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_ValidaDominio;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_VDM_ValidaDominioTests : BaseTest
{
    [SetUp]
    public async Task SetupValidaDominio()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "Valida dominio" }), "Clic en Valida Dominio");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    // Usamos TestCase para probar múltiples dominios con un solo método
    [TestCase("axelinares.com", true, TestName = "Dominio Válido (.com)")]
    [TestCase("axelinares.com.mx", true, TestName = "Dominio Válido (.com.mx)")]
    [TestCase("https://axelinares.com.mx", false, TestName = "Dominio Inválido (Contiene https)")]
    [TestCase("facebook.com", false, TestName = "Dominio Inválido (Terminación no admitida/reservada)")]
    public async Task QA_VDM_ValidarReglasDeNegocioDominios(string dominioAProbar, bool debeSerValido)
    {
        var inputDominio = Page.GetByRole(AriaRole.Textbox, new() { Name = "Ejemplo: tudominio.com" });
        var btnComprobar = Page.GetByText("Comprobar Disponibilidad");

        await inputDominio.FillAsync(dominioAProbar);
        await ClickConMonitoreo(btnComprobar, $"Comprobando: {dominioAProbar}");

        if (debeSerValido)
        {
            // Validar que NO aparezca el mensaje de error de caracteres especiales o terminación inválida
            await Expect(Page.GetByText("Por favor utilice puntos en su dominio")).ToBeHiddenAsync();
            await Expect(Page.GetByText("La terminación del dominio no es valida")).ToBeHiddenAsync();
        }
        else
        {
            // Validar que aparezca algún mensaje de error visible
            var mensajeError1 = Page.GetByText("Por favor utilice puntos en su dominio. Evite http y caracteres especiales");
            var mensajeError2 = Page.GetByText("La terminación del dominio no es valida");
            
            // Verificamos que al menos uno de los dos errores esté visible en pantalla
            bool errorVisible = await mensajeError1.IsVisibleAsync() || await mensajeError2.IsVisibleAsync();
            Assert.That(errorVisible, Is.True, $"Se esperaba un error para el dominio {dominioAProbar}, pero fue aceptado.");
        }
    }

    [TearDown]
    public async Task TeardownValidaDominio()
    {
        // Regresar al dashboard al terminar cada caso
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}