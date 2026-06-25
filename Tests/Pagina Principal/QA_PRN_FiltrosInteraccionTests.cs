using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using System;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: INTERACCIÓN Y FILTROS (DASHBOARD)
// ---------------------------------------------------------
/// <summary>
/// Ejecuta pruebas de estrés UI sobre los elementos interactivos de la página principal 
/// (pestañas, filtros dinámicos y algoritmos de búsqueda principal).
/// </summary>
[TestFixture]
[Category("Dashboard")]  
[Category("Sanity")]     
[Category("Media")]      
public class QA_PRN_FiltrosInteraccionTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_PRN_11_al_13_NavegacionPorPestanas()
    {
        LogWriter("Iniciando validación de flujo de enrutamiento interno (Pestañas).");
        var pestanas = new[] { "Cartera", "Pendientes", "Tablero" };

        foreach (var pestana in pestanas)
        {
            var selector = Page.GetByRole(AriaRole.Link, new() { Name = pestana, Exact = true });
            await ClickConMonitoreo(selector, $"Pestaña {pestana}");
            
            // Verificamos que el contenedor general se mantenga estable en el DOM
            await Expect(Page.Locator(".content-wrapper").First).ToBeVisibleAsync();
        }
    }

    [Test]
    public async Task QA_PRN_15_al_17_FiltrosDeSinergia()
    {
        LogWriter("Iniciando pruebas de renderizado por filtros de Sinergia.");
        var filtros = new[] { "Residencial", "Ambos", "Comercial" };

        foreach (var filtro in filtros)
        {
            LogWriter($"Inyectando estado de filtro: {filtro}");
            var selector = Page.Locator($"text='{filtro}'").First;
            
            await ClickConMonitoreo(selector, $"Botón Filtro {filtro}");
            await Expect(Page.Locator("#pptoComercial")).ToBeAttachedAsync();
        }
    }

    [Test]
    public async Task QA_PRN_18_BuscadorVacio()
    {
        IgnorarCortafuegosAlertas = true;
        LogWriter("Ejecutando prueba negativa en el motor de búsqueda principal (Manejo de excepciones visuales).");
        var inputBuscador = Page.GetByPlaceholder("buscar...");
        
        // Semilla dinámica para garantizar cero colisiones de datos
        string textoFalso = "PruebaQA_" + DateTime.Now.Ticks;
        await inputBuscador.FillAsync(textoFalso);

        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).First, "Ejecutar Búsqueda");

        // Aserción directa de prevención de caídas de front-end
        await Expect(Page.GetByText("No se encontraron resultados", new() { Exact = false }).First).ToBeVisibleAsync();
        LogWriter("El sistema manejó la excepción de datos vacíos de forma elegante.");
    }
}