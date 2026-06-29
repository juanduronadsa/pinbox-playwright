using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Herramientas;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_HerramientasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_AYD_ValidarVisibilidadEnlacesHerramientas()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Herramientas" }), "Clic en Herramientas");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // ================================================================
        // 2. VALIDACIÓN DE RED INTERNA (Solo visibilidad)
        // ================================================================
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Tab, new() { Name = "Red Interna" }), "Desplegar Red Interna");
        
        // Verificamos que los textos/enlaces existan en el DOM y sean visibles para el usuario SIN dar clic
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "IAM" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Stock Fotos" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Intranet" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "911 sistemas" })).ToBeVisibleAsync();

        // ================================================================
        // 3. VALIDACIÓN DE RED EXTERNA (Solo visibilidad)
        // ================================================================
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Tab, new() { Name = "Red externa" }), "Desplegar Red Externa");
        
        // Verificamos una muestra representativa de los enlaces externos
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Twitter ADSA" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Facebook ADSA" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Políticas de Google ADS" })).ToBeVisibleAsync();

        // 4. Limpieza: Retorno seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}