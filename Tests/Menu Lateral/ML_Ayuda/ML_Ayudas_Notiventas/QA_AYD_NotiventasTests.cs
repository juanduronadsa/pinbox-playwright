using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Notiventas;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_NotiventasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_AYD_AperturaNoticiaEnNuevaPestana()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.Locator("#menu_notiventas"), "Clic en Notiventas");

        // Manejo de Pop-up (Nueva pestaña)
        var nuevaPestana = await Page.RunAndWaitForPopupAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Cell).First.ClickAsync();
        });

        Assert.That(nuevaPestana, Is.Not.Null, "El documento de Notiventas no se abrió.");
        await nuevaPestana.CloseAsync(); // Cerramos la pestaña generada

        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}