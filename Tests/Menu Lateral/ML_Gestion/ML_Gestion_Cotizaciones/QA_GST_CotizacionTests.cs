using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Playwrigt_Demo.Factories;
using Playwrigt_Demo.Models;
using System;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: COTIZADOR MASIVO (Módulo 4.1)
// ---------------------------------------------------------
[TestFixture]
[Ignore("Revisión pendiente: Módulo excluido temporalmente para análisis de reportes.")]
[Category("Cotizacion")] 
[Category("Regresion")]  
[Category("Critica")]  
public class QA_GST_CotizacionTests : BaseTest
{
    // ---------------------------------------------------------
    // HELPERS Y ALIMENTADORES DE DATOS
    // ---------------------------------------------------------
    public static IEnumerable<TestCaseData> LeerCasosCotizacion()
    {
        // 🚨 CAMBIO CRÍTICO: Apuntar al nuevo archivo de datos
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_GST_CotizacionData.json");
        
        // 🚨 CORTAFUEGOS: Si no encuentra el archivo, lanza una prueba fallida en lugar de desaparecer
        if (!File.Exists(ruta))
        {
            yield return new TestCaseData(new CotizacionTestData()).SetName("ERROR_NO_SE_ENCONTRO_EL_JSON_DE_COTIZACIONES");
            yield break;
        }

        var json = File.ReadAllText(ruta);
        var casos = JsonSerializer.Deserialize<List<CotizacionTestData>>(json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (casos != null)
        {
            foreach (var caso in casos)
            {
                string prodName = string.IsNullOrEmpty(caso.Producto) ? "SinProducto" : caso.Producto.Replace(" ", "");
                yield return new TestCaseData(caso).SetName($"{caso.CasoId}_{prodName}");
            }
        }
    }

    private async Task ConfigurarProductoInteligente(CotizacionTestData datos)
    {
        var config = ProductoFactory.ObtenerConfiguracion(datos.Producto);
        
        // 🚨 CORRECCIÓN DE COMPILADOR: Agregamos el return para asegurar la detención si es nulo
        if (config == null) 
        {
            Assert.Fail($"Producto '{datos.Producto}' no encontrado en el Catálogo Maestro.");
            return; 
        }

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Seleccione Producto" }).ClickAsync();
        await Page.GetByText(datos.Producto).First.ClickAsync();

        if (config.PeriodosValidos.Any())
            await SeleccionarEnSelectize("Seleccione Periodo", datos.Periodo);

        if (config.SuscripcionesValidas.Any())
            await SeleccionarEnSelectize("Selecciones suscripción", datos.Suscripcion);

        await SeleccionarEnSelectize("Seleccione pago", datos.MetodoPago);
    }

    private async Task SeleccionarEnSelectize(string placeholder, string valor)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = placeholder }).ClickAsync();
        await Page.GetByText(valor, new() { Exact = true }).First.ClickAsync();
    }

    [Test]
    [TestCaseSource(nameof(LeerCasosCotizacion))]
    public async Task QA_CTZ_02_CreacionCompletaDataDriven(CotizacionTestData testCase)
    {
        var cliente = ClientePoolFactory.ObtenerClienteEnTurno();
        await LoginEspecifico(cliente.UsuarioPropietario, cliente.PasswordPropietario);

        LogWriter($"Cotizando '{testCase.Producto}' para Cliente: {cliente.Nombre} usando Agente: {cliente.UsuarioPropietario}");

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        
        await Task.Delay(1000); 
        await Page.GetByRole(AriaRole.Link, new() { Name = "Cotizador" }).ClickAsync(); 
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        LogWriter($"Buscando cliente por nombre: {cliente.Nombre}...");
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Buscar cliente" }).FillAsync(cliente.Nombre);
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Buscar" }), "Botón Buscar Cliente");
        
        var filaCliente = Page.Locator("tr").Filter(new() { HasText = cliente.Nombre }).First;
        
        try 
        {
            // 🚨 60 segundos EXCLUSIVOS para darle tiempo a la API del buscador
            await filaCliente.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 60000 });
        }
        catch (System.TimeoutException)
        {
            Assert.Fail($"[ERROR DE AMBIENTE] La API de búsqueda colapsó. El cliente {cliente.Nombre} no se cargó en la tabla tras 60 segundos.");
            return;
        }

        LogWriter("Validando integridad de datos cruzados...");
        await Expect(filaCliente).ToContainTextAsync(cliente.IdCliente);

        await filaCliente.GetByRole(AriaRole.Link, new() { Name = "Cotizador" }).ClickAsync();

        await ConfigurarProductoInteligente(testCase);

        await SeleccionarEnSelectize("Estado", cliente.Estado);
        await SeleccionarEnSelectize("Poblacion", cliente.Municipio);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Teléfono" }).FillAsync(cliente.Telefono);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Correo" }).FillAsync(cliente.Correo);
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Seleccione Categoría" }).FillAsync(cliente.Categoria);
        await Page.GetByText(cliente.Categoria, new() { Exact = false }).First.ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Agregar Producto" }).ClickAsync();
        LogWriter("Cotización completada exitosamente.");
    }

    [Test]
    public async Task QA_CTZ_03_ValidacionCamposVacios()
    {
        LogWriter("Iniciando QA-CTZ-03: Prueba negativa en Cotizador.");
        
        var cliente = ClientePoolFactory.ObtenerClienteAleatorio();
        await LoginEspecifico(cliente.UsuarioPropietario, cliente.PasswordPropietario);
        
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000); 
        await Page.GetByRole(AriaRole.Link, new() { Name = "Cotizador" }).ClickAsync(); 
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Buscar cliente" }).FillAsync(cliente.Nombre);
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Buscar" }), "Botón Buscar Cliente");

        var filaCliente = Page.Locator("tr").Filter(new() { HasText = cliente.Nombre }).First;
        
        try 
        {
            // 🚨 60 segundos EXCLUSIVOS también aquí, porque es la misma consulta pesada
            await filaCliente.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 60000 });
        }
        catch (System.TimeoutException)
        {
            Assert.Fail($"[ERROR DE AMBIENTE] La API de búsqueda colapsó. El cliente {cliente.Nombre} no se cargó tras 60 segundos.");
            return;
        }

        await filaCliente.GetByRole(AriaRole.Link, new() { Name = "Cotizador" }).ClickAsync();

        LogWriter("Forzando adición de producto vacío...");
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Agregar Producto" }), "Agregar sin datos");

        await Expect(Page.GetByText("Seleccione", new() { Exact = false }).First).ToBeVisibleAsync();
        
        LogWriter("Validación de campos vacíos en cotizador exitosa.");
    }
}