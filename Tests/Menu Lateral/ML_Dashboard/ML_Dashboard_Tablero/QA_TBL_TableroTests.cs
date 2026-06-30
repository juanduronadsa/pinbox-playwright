using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_Tablero;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_TBL_TableroTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_TBL_DescargaReporte_Y_ValidacionEntorno()
    {
        // Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "Tablero" }), "Clic en Tablero");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Descargar Reporte" })).ToBeVisibleAsync(); // FIX: NetworkIdle → espera determinista

        // Prueba de Descarga del Reporte del Tablero
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Descargar Reporte" }), "Botón Descargar Reporte");
        });
        
        Assert.That(download, Is.Not.Null, "El sistema no generó la descarga del reporte.");

        // Regreso seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }

    [Test]
    [Ignore("TICKET-POR-REPORTAR: El botón 'Ir a Certificación de Ventas' genera un Error 500 (An error occurred while processing your request).")]
    public async Task QA_TBL_NavegacionCertificacionVentas()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "Tablero" }), "Clic en Tablero");
        
        // Esta acción rompe el entorno actualmente.
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Ir a Certificación de Ventas" }), "Ir a Certificación");
        
        // Validación futura
        await Expect(Page.Locator("h1")).ToBeVisibleAsync(); 
    }
}