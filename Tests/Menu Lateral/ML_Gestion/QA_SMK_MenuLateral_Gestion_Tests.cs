using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: MENÚ LATERAL - GESTIÓN
// ---------------------------------------------------------
/// <summary>
/// Navega por los módulos críticos de ML Gestión (Creación, Cotizador).
/// No ejecuta los flujos completos, sino que comprueba que los módulos estén disponibles.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Sanity")]
public class QA_SMK_MenuLateral_Gestion_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuGestion()
    {
        await LoginDinamico();
        await ClickConMonitoreo(Page.Locator("#tab-home-2"), "Cambio a Pestaña Gestión");
    }

    // 🚨 CAMBIO 1: Se agregó el parámetro opcional 'timeoutMs' con tu valor por defecto de 10000
    private async Task NavegarYRegresarGestion(string textoEnlace, string selectorValidacion, int timeoutMs = 10000)
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");

        var opcionMenu = Page.Locator($"a:has-text('{textoEnlace}')").First;
        await ClickConMonitoreo(opcionMenu, $"Selección de {textoEnlace}");

        try 
        {
            // Sincronización explícita sobre el DOM del destino
            // 🚨 CAMBIO 2: Usamos la variable 'timeoutMs' en lugar del número duro
            await Page.Locator(selectorValidacion).First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeoutMs });
            
            var btnBack = Page.Locator("#back, #back a").First;
            if (await btnBack.IsVisibleAsync()) await btnBack.ClickAsync();
        }
        catch (System.TimeoutException)
        {
            Assert.Fail($"[ERROR DE RUTA] No se cargó la vista '{textoEnlace}' correctamente. Selector no encontrado: {selectorValidacion}");
        }
    }

    [Test]
    public async Task QA_SMK_03_Redireccion_ClienteNuevo() => await NavegarYRegresarGestion("Cliente nuevo", "text='CREAR CLIENTE NUEVO'");

    [Test]
    // 🚨 CAMBIO 3: Le inyectamos 15000 milisegundos (15 segundos) para esperar al loader del Cotizador
    public async Task QA_SMK_03_Redireccion_Cotizador() => await NavegarYRegresarGestion("Cotizador", "text='Nueva cotización'", 15000);
}