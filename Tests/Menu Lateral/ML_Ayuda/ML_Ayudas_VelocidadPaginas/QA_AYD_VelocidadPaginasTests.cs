using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_VelocidadPaginas;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_VelocidadPaginasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_AYD_RedireccionPageSpeed()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");

        // Valida la redirección a una herramienta externa de Google
        var popupPageSpeed = await Page.RunAndWaitForPopupAsync(async () =>
        {
            await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Velocidad de páginas" }), "Clic en Velocidad");
        });

        await Expect(popupPageSpeed).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("pagespeed"));
        await popupPageSpeed.CloseAsync();
    }
}