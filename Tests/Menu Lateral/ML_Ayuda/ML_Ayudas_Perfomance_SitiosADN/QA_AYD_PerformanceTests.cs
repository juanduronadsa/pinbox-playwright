using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Performance;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_PerformanceTests : BaseTest
{
    [Test]
    public async Task QA_AYD_RenderizadoPowerBI()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.GetByTitle("Performance"), "Clic en Performance");

        // Localizar el iframe de PowerBI y asegurar que cargue
        var iframePowerBI = Page.FrameLocator("iframe").First;
        await Expect(iframePowerBI.Locator("div").First).ToBeVisibleAsync();

        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}