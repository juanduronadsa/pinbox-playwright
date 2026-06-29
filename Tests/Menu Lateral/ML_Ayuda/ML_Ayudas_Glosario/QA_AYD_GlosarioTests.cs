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
    }

    [Test]
    public async Task QA_AYD_NavegacionGlosario()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByTitle("Glosario"), "Clic en Glosario");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 2. Validación del filtro alfabético dinámico (Letra I)
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = "I", Exact = true }), "Filtro Letra I");
        
        // 3. Verificamos que el concepto correspondiente se muestre en pantalla
        await Expect(Page.GetByText("In progress: En proceso de")).ToBeVisibleAsync();

        // 4. Limpieza: Retorno seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}