using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Playwrigt_Demo.Tests.Menu_Lateral.ML_Ayuda.ML_Ayudas_Productos;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Ayudas")]
[Category("Funcional")]
public class QA_AYD_ProductosTests : BaseTest
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
    public async Task QA_AYD_FiltrosCheckboxProductos()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Productos" }), "Clic en Productos");
        await Expect(Page.GetByRole(AriaRole.Checkbox).First).ToBeVisibleAsync(); // FIX: NetworkIdle → espera determinista del primer checkbox del catálogo

        // 2. Interacción con el catálogo de productos
        // Filtramos para encontrar el checkbox específico que mostraste en tu grabación
        var checkboxFiltro = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "EstéticosTelas del" }).GetByRole(AriaRole.Checkbox);
        
        // La acción CheckAsync() es la forma más nativa y segura en Playwright de interactuar con un input type="checkbox"
        await checkboxFiltro.CheckAsync();
        
        // 3. Validación de estado (Aserción de que el click realmente funcionó)
        await Expect(checkboxFiltro).ToBeCheckedAsync();

        // 4. Limpieza: Retorno seguro
        await ClickConMonitoreo(Page.Locator("#back, #back a").First, "Regresar al menú principal");
    }
}