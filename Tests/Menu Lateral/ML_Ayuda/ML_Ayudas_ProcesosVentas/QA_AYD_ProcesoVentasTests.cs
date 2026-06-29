using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayuda.ML_Ayudas_ProcesosVentas;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas")]
[Category("Funcional")]
public class QA_AYD_ProcesoVentasTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_AYD_NavegacionAcordeonVentas()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Proceso de ventas" }), "Clic en Proceso de Ventas");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 2. Iteración por acordeones/pestañas
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Tab, new() { Name = "Preparación Previsita" }), "Desplegar Pestaña Previsita");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Tab, new() { Name = "Tarifas" }), "Desplegar Pestaña Tarifas");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Tab, new() { Name = "Presentación", Exact = true }), "Desplegar Pestaña Presentación");

        // 3. Limpieza: Retorno seguro al Dashboard
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}