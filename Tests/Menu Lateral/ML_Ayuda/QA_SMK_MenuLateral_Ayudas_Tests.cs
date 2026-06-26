using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: MENÚ LATERAL - AYUDAS
// ---------------------------------------------------------
/// <summary>
/// Valida la navegación de los enlaces ubicados en el menú "Ayudas" utilizando una 
/// estrategia de muestreo. Comprueba enlaces internos y externos (Popups).
/// </summary>
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
public class QA_SMK_MenuLateral_Ayudas_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuAyudas()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }

    public static IEnumerable<TestCaseData> LeerLinksAyudaMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_SMK_AyudasLinksData.json");
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(File.ReadAllText(rutaJson))!;

        string[] idsMuestra = { "QA-SMK-04.1", "QA-SMK-04.5", "QA-SMK-04.11" };
        var muestra = enlacesCompletos.Where(e => idsMuestra.Contains(e.Id));

        foreach (var link in muestra)
        {
            yield return new TestCaseData(link).SetName($"{link.Id}_{link.TextoEnlace.Replace(" ", "")}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksAyudaMuestreo))]
    public async Task QA_SMK_04_Barredor_Ayudas_Dinamico(LinkTestData datos)
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú");
        
        var opcionMenu = Page.Locator($"a:has-text('{datos.TextoEnlace}')").First;

        // ====================================================================
        // 🛡️ NUEVA LÓGICA: Desplegar el acordeón padre si la opción está oculta
        // ====================================================================
        if (!await opcionMenu.IsVisibleAsync())
        {
            LogWriter("El enlace está oculto en el acordeón. Abriendo categoría 'Ayudas'...");
            
            // Buscamos el elemento padre "Ayudas" y le damos clic para desplegarlo
            // Nota: Si "Ayudas" es un botón o tiene otra clase, puedes ajustar este selector
            await Page.GetByText("Ayudas", new() { Exact = true }).First.ClickAsync();
            
            // Espera breve para permitir que la animación CSS termine de bajar el submenú
            await Task.Delay(1000); 
        }
        // ====================================================================

        // Lógica de bifurcación: Manejo de Popups Externos vs Ruteo Interno
        if (datos.EsExterno)
        {
            LogWriter($"Interceptando evento de nueva pestaña (Target='_blank') para {datos.TextoEnlace}");
            var popup = await Page.RunAndWaitForPopupAsync(async () => {
                // Clic natural, ya sin Force=true porque el elemento es visible
                await opcionMenu.ClickAsync();
            });
            await popup.CloseAsync();
        }
        else
        {
            LogWriter($"Navegación Interna {datos.TextoEnlace}");
            
            // Clic natural, ya sin Force=true porque el elemento es visible
            await opcionMenu.ClickAsync();
            
            await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

            var btnRegresar = Page.Locator("#back, #back a").First;
            if (await btnRegresar.IsVisibleAsync()) await btnRegresar.ClickAsync();
        }
    }
}