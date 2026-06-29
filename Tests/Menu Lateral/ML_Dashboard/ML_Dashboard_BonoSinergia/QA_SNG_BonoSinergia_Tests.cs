using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_BonoSinergia;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_SNG_BonoSinergia_Tests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    [Ignore("TICKET-POR-REPORTAR: El entorno de pruebas devuelve Error 500 (The view 'bonosinergia' was not found).")]
    public async Task QA_SNG_ValidacionBonoSinergia()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        
        // 2. Clic en el módulo afectado
        await ClickConMonitoreo(Page.Locator("a").Filter(new() { HasText = "Bono Sinergia" }).First, "Clic en Bono Sinergia");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // 3. Espacio reservado para aserciones futuras cuando el entorno esté funcional
        await Expect(Page.GetByRole(AriaRole.Heading)).ToBeVisibleAsync(); 
    }
}