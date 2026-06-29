using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_AppPinbox;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_AppPinboxTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_AYD_ValidarDescargaAPK()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.GetByTitle("AppPinbox"), "Clic en App Pinbox");

        // Validar intercepción de descarga
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "Descargar" }).First, "Descargar APK");
        });
        
        Assert.That(download, Is.Not.Null, "La descarga de la app falló.");
        
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}