using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: MENÚ LATERAL - DASHBOARD (SMOKE)
// ---------------------------------------------------------
[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]
public class QA_SMK_MenuLateral_Dashboard_Tests : BaseTest
{
    [SetUp]
    public async Task SetupMenuDashboard()
    {
        // Navegación base asegurada antes de cada test de la lista
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        await Expect(Page.Locator("#tab-home-1")).ToBeVisibleAsync(); // FIX: NetworkIdle → tab home visible
    }

    public static IEnumerable<TestCaseData> LeerLinksTableroMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_SMK_TableroLinksData.json");
        var enlacesCompletos = JsonSerializer.Deserialize<List<LinkTestData>>(File.ReadAllText(rutaJson))!;

        // Ejecutar dinámicamente solo los que estén habilitados
        var muestraActiva = enlacesCompletos.Where(e => e.Habilitado);

        foreach (var link in muestraActiva)
        {
            yield return new TestCaseData(link).SetName($"{link.Id}_Navegacion_{link.TextoEnlace.Replace(" ", "")}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerLinksTableroMuestreo))]
    public async Task QA_SMK_Dashboard_NavegacionMenuLateral(LinkTestData datos)
    {
        // 1. Desplegar el menú lateral
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        
        // 2. Clic dinámico a la opción del menú extraída del JSON
        // Nota: Usamos ContainsText para evadir los íconos (ej. ' Comisiones por resultados')
        var opcionMenu = Page.Locator("a").Filter(new() { HasText = datos.TextoEnlace }).First;
        await ClickConMonitoreo(opcionMenu, $"Navegación a {datos.TextoEnlace}");

        // 3. Validar que la pantalla destino cargó correctamente su título
        LogWriter($"Verificando renderizado del título en destino: {datos.SelectorValidacion}");
        // FIX: NetworkIdle eliminado — el Expect siguiente ya tiene auto-espera de 45s
        await Expect(Page.Locator(datos.SelectorValidacion).First).ToBeVisibleAsync();

        // 4. Retorno seguro al Dashboard
        var botonRegresar = Page.Locator("#back, #back a").First;
        if (await botonRegresar.IsVisibleAsync()) 
        {
            await ClickConMonitoreo(botonRegresar, "Botón Regresar al Dashboard");
            // Confirmamos que el retorno fue exitoso esperando el contenedor de datos principal
            await Expect(Page.Locator("#contenedorDatos").First).ToBeVisibleAsync();
        }
    }
}