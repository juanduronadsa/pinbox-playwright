using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_PreparadorDigital;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_PreparadorDigitalTests : BaseTest
{
    [Test]
    public async Task QA_AYD_FlujoFormularioPreparador()
    {
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Preparador digital" }), "Clic en Preparador");

        // Llenado de formulario
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "seccion amarilla" }).FillAsync("hoteles");
        await Page.Locator("#inpGoogle").CheckAsync();
        
        await ClickConMonitoreo(Page.GetByText("Verificar redes sociales").First, "Botón Verificar");

        // Regreso seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar");
    }
}