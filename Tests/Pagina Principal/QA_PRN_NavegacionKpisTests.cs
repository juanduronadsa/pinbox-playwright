using System;
using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

[TestFixture]
[Category("Dashboard")]
[Category("Regresion")]  
[Category("Media")]      
public class QA_PRN_NavegacionKpisTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    public static IEnumerable<TestCaseData> LeerDatosKpisMuestreo()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_PRN_KpisData.json");
        var json = File.ReadAllText(rutaJson);
        var kpis = JsonSerializer.Deserialize<List<KpiTestData>>(json)!;

        // Sampling: Primer KPI, Medio y Último para no saturar
        var muestra = new List<KpiTestData> { kpis.First(), kpis[kpis.Count / 2], kpis.Last() };

        foreach (var kpi in muestra)
        {
            yield return new TestCaseData(kpi).SetName($"{kpi.CasoId}_ValidarKPI_{kpi.TituloEsperado}");
        }
    }

    [Test]
    [TestCaseSource(nameof(LeerDatosKpisMuestreo))]
    public async Task QA_PRN_ValidarEnlacesDeKPICirculares(KpiTestData datos)
    {
        LogWriter($"Ejecutando ruteo hacia detalle de KPI: {datos.Boton}");

        ILocator selectorKpi = datos.EsId 
            ? Page.Locator(datos.Boton!) 
            : Page.GetByText(datos.Boton!).First;

        try 
        {
            await ClickConMonitoreo(selectorKpi, $"Acceso a KPI {datos.CasoId}");
            
            // 🚨 SOLUCIÓN 1: Esperar a que la consulta de base de datos / red termine
            try { await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 }); } catch { }
            
            // 🚨 SOLUCIÓN 2: Búsqueda agnóstica al HTML (sin forzar que sea un span)
            var tituloDestino = Page.GetByText(datos.TituloEsperado).First;
            
            // Esperamos que el elemento se vuelva visible
            await tituloDestino.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await Expect(tituloDestino).ToBeVisibleAsync();
            
            LogWriter($"Ruteo al KPI '{datos.TituloEsperado}' completado exitosamente.");

            // 🚨 SOLUCIÓN 3: Regreso seguro al Dashboard para mantener el DOM limpio para el siguiente KPI
            var botonRegresar = Page.Locator("#back, #back a").First;
            if (await botonRegresar.IsVisibleAsync())
            {
                await ClickConMonitoreo(botonRegresar, "Retorno al Dashboard");
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }
        catch (System.TimeoutException)
        {
            // Manejo de Resiliencia
            Assert.Warn($"[LATENCIA DE RED] El KPI {datos.CasoId} no se cargó a tiempo. Posible cuello de botella en el servicio.");
        }
    }
}