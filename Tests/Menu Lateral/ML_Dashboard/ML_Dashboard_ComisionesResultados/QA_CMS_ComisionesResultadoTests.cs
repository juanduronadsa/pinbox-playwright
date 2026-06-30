using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Dashboard.ML_Dashboard_Comisiones;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: DASHBOARD - COMISIONES POR RESULTADOS
// ---------------------------------------------------------
[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Dashboard")]
[Category("Funcional")]
public class QA_CMS_ComisionesResultadoTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
    }
    [Test]
    public async Task QA_CMS_IntegridadDatosTodasLasComisiones()
    {
        // 1. Navegación al módulo
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura Menú Lateral");
        await ClickConMonitoreo(Page.Locator("a").Filter(new() { HasText = "Comisiones por resultados" }).First, "Clic en Comisiones por resultados");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Reporte de comisiones por resultados" })).ToBeVisibleAsync();

        // 2. Seleccionar el periodo 12 (Diciembre) para forzar la aparición de la Comisión Anual
        await Page.Locator("#dropPeriods").SelectOptionAsync(new[] { "12" });
        await Expect(Page.Locator(".card, table tbody tr, [class*=tarjeta]").First).ToBeVisibleAsync(new() { Timeout = 20000 }); // FIX: NetworkIdle → espera determinista de tarjetas

        // Patrón para extraer moneda: $1,167,285.00 o $0.00
        string regexMoneda = @"\$[0-9]{1,3}(?:,[0-9]{3})*(?:\.[0-9]{2})";

        // 3. Arreglo con los identificadores únicos de las 5 posibles tarjetas
        string[] tiposDeComision = {
            "Comision Anual por Crecimiento",
            "Comision mensual por Resultados",
            "Trimestral por Nivel de Certificacion",
            "Renovacion de Campa", // Evitamos la "ñ" por temas de codificación en el DOM
            "Bonos Sinergias"
        };

        // 4. Iteración dinámica: El robot actuará según lo que esté renderizado
        foreach (var tipo in tiposDeComision)
        {
            var tarjeta = Page.Locator("div.card, div[class*='col-']").Filter(new() { HasText = tipo }).First;

            // Cortafuegos: Si la tarjeta no existe en este periodo, saltamos a la siguiente
            if (await tarjeta.IsVisibleAsync())
            {
                LogWriter($"Validando tarjeta: {tipo}");

                // Extraemos el texto completo de la tarjeta y sacamos el monto con Regex
                string textoTarjeta = await tarjeta.InnerTextAsync();
                Match match = Regex.Match(textoTarjeta, regexMoneda);
                
                Assert.That(match.Success, Is.True, $"No se encontró un formato de moneda válido en la tarjeta: {tipo}");
                string montoExtraido = match.Value;

                // Clic para ver el detalle de ESTA tarjeta en específico
                await ClickConMonitoreo(tarjeta.GetByRole(AriaRole.Button, new() { Name = "Click aqui para ver detalle" }), $"Ver detalle de {tipo}");
                await Expect(Page.Locator("body")).ToBeVisibleAsync(); // FIX: NetworkIdle → el monto se valida en el Expect inmediato siguiente

                // Validación de integridad: Buscamos que el monto exacto exista dentro del contenedor de detalles
                // Usamos una aserción genérica en el body o contenedor principal para asegurar que el dato cruzó bien
                var elementoMontoDetalle = Page.Locator("body").Filter(new() { HasText = montoExtraido }).First;
                await Expect(elementoMontoDetalle).ToBeVisibleAsync();

                LogWriter($"Integridad confirmada para {tipo} con el monto {montoExtraido}");

                // Retorno al grid de tarjetas
                await ClickConMonitoreo(Page.Locator("#back a, #back").First, "Regresar a reporte de comisiones");
                await Expect(Page.Locator(".tarjeta, .card, table tbody tr").First).ToBeVisibleAsync(new() { Timeout = 15000 }); // FIX: esperar que las tarjetas reaparezcan
            }
        }
    }
}