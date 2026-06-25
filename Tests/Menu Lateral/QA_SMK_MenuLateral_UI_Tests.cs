using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTERFAZ GRÁFICA DEL MENÚ (UI)
// ---------------------------------------------------------
/// <summary>
/// Pruebas atómicas enfocadas en los micro-componentes de la UI. 
/// Válida colapsos, animaciones y limpieza del DOM.
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
[Category("Media")]
public class QA_SMK_MenuLateral_UI_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuUI()
    {
        await LoginDinamico();
        LogWriter("Configurando punto de partida en Pestaña Dashboard.");
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }

    [Test]
    public async Task QA_SMK_01_AperturaCierreHamburguesa()
    {
        LogWriter("Iniciando Prueba atómica: Apertura y Cierre del Menú usando el mismo botón.");
    
        // Identificamos el botón único (el de la hamburguesa)
        var botonToggle = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        await ClickConMonitoreo(botonToggle, "Apertura de Menú Lateral");
        // Verificamos que el menú se abrió (el link 'Salir' aparece)
        await Expect(Page.Locator("a:has-text('Salir')").First).ToBeVisibleAsync();
        
        // Nota: A veces el DOM cambia el nombre del botón de "Open Menu" a "Close Menu" 
        // al estar abierto. Si es el mismo elemento, usamos el mismo locator.
        await ClickConMonitoreo(botonToggle, "Cierre de Menú Lateral");
    
        // Validamos que el menú se cerró (el link 'Salir' se oculta)
        await Expect(Page.Locator("a:has-text('Salir')").First).ToBeHiddenAsync(new() { Timeout = 3000 });
    
        LogWriter("Apertura y Cierre con botón único validados correctamente.");
    }
}