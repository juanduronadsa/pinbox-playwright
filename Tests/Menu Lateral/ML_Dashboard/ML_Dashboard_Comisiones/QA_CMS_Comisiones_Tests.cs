using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_Comisiones;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_CMS_Comisiones_Tests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }    
    [Test]
    public async Task QA_CMS_FlujoDescargaExcel_Y_Aclaracion()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Comisiones" }), "Clic en Comisiones");
        await Expect(Page.Locator("#dropPeriodos")).ToBeVisibleAsync(); // FIX: NetworkIdle → dropdown de periodos visible

        // 2. Selección de periodo
        await Page.Locator("#dropPeriodos").SelectOptionAsync(new[] { "2025-22" });
        await Expect(Page.Locator("table tbody tr").First).ToBeVisibleAsync(new() { Timeout = 20000 }); // FIX: tabla cargada tras seleccionar periodo

        // 3. Interacción con la tabla (Selecciona la primera fila dinámicamente)
        var filaDesplegable = Page.Locator("table tbody tr").First.Locator("a").First;
        if (await filaDesplegable.IsVisibleAsync())
        {
            await filaDesplegable.ClickAsync();
        }

        // 4. Prueba de Descarga de Excel
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await ClickConMonitoreo(Page.Locator("#linkExcel"), "Botón Descargar Excel");
        });
        
        Assert.That(download, Is.Not.Null, "El sistema no generó la descarga del Excel.");
        Assert.That(download.SuggestedFilename, Does.EndWith(".xls").Or.EndWith(".xlsx"), "El archivo descargado no es un Excel válido.");

        // 5. Flujo de Aclaración
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Aclaración" }), "Botón Aclaración");
        await Expect(Page.Locator("#mySelect")).ToBeVisibleAsync(); // FIX: NetworkIdle → panel de aclaración visible

        await Page.Locator("#mySelect").SelectOptionAsync(new[] { "1" });
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Advertiser" }).FillAsync("1");
        await ClickConMonitoreo(Page.Locator("#buscar"), "Botón Buscar Aclaración");
        
        // Validación final
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "SOLO ANUNCIANTES DE TU" })).ToBeVisibleAsync();

        // Limpieza: Retorno seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú anterior");
    }
}