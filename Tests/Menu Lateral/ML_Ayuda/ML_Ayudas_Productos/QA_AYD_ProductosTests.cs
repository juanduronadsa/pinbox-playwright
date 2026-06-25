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
    [Test]
    public async Task QA_AYD_FiltrosCheckboxProductos()
    {
        // 1. Navegación base
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Abrir Menú Lateral");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Link, new() { Name = " Productos" }), "Clic en Productos");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

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