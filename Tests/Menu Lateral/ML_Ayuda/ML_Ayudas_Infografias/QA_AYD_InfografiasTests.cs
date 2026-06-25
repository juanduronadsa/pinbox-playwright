using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Infografias;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_InfografiasTests : BaseTest
{
    [Test]
    public async Task QA_AYD_FiltroYVisualizacionInfografias()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.Locator("#menu_infografia"), "Clic en Infografías");

        await Page.Locator("#dropImg").SelectOptionAsync(new[] { "MARZO_2016.jpg" });
        await Expect(Page.Locator("#imgInfo")).ToBeVisibleAsync();

        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}