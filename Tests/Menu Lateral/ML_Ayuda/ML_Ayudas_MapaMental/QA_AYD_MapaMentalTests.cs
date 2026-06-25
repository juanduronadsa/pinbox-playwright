using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_MapaMental;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_MapaMentalTests : BaseTest
{
    [Test]
    public async Task QA_AYD_CargaVisualMapaMental()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.Locator("#menu_mapamental"), "Clic en Mapa Mental");

        await Expect(Page.GetByRole(AriaRole.Img).First).ToBeVisibleAsync();
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}