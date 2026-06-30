using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_CuantoFaltaComisiones;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_CFC_CuantoFaltaComisionesTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_CFC_ValidarTextosInformativos()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "¿Cuánto falta para mis comisiones por resultados?" }), "Clic en Cuanto Falta");
        await Expect(Page.GetByText("Nota: los contratos ingresados tardan en promedio 3 días hábiles")).ToBeVisibleAsync(); // FIX: NetworkIdle → texto estático de la pantalla

        // Validar textos estáticos principales
        await Expect(Page.GetByText("Nota: los contratos ingresados tardan en promedio 3 días hábiles")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Por el momento no cuenta con información de comisiones")).ToBeVisibleAsync();

        // Regreso seguro
        await ClickConMonitoreo(Page.Locator("#back a, #back").First, "Regresar al menú principal");
    }
}