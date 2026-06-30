using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayudas.ML_Ayudas_Glosario;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas"), Category("Funcional")]
public class QA_AYD_GlosarioTests : BaseTest
{
    [SetUp]
    public async Task ConfiguracionInicial()
    {
        await LoginDinamico();
        await Page.Locator("#tab-home-1").ClickAsync(new() { Force = true });
        // 🚨 FIX (codegen): el menú lateral muestra Gestión por defecto. Los links de Ayudas
        // no aparecen en el DOM hasta cambiar a esta sección — causa del timeout 10s.
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Ayudas" }).CheckAsync();
    }

    [Test]
    public async Task QA_AYD_NavegacionGlosario()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByTitle("Glosario"), "Clic en Glosario");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "I", Exact = true })).ToBeVisibleAsync(); // FIX: NetworkIdle → espera determinista del filtro alfabético

        // 2. Validación del filtro alfabético dinámico (Letra I)
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "I", Exact = true }), "Filtro Letra I");
        
        // 3. Verificamos que el concepto correspondiente se muestre en pantalla
        await Expect(Page.GetByText("In progress: En proceso de")).ToBeVisibleAsync();

        // 4. Limpieza: Retorno seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}