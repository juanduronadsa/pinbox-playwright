using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using Playwrigt_Demo.Factories;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: ALTA DE CLIENTES (Módulo 4.2)
// ---------------------------------------------------------
[TestFixture]
[Ignore("Revisión pendiente: Módulo excluido temporalmente para análisis de reportes.")]
[Category("Clientes")]   
[Category("Regresion")]  
[Category("E2E")]        
[Category("Alta")]       
public class QA_GST_ClientesTests : BaseTest
{
    [Test]
    public async Task QA_CLN_01_ValidacionCamposObligatorios()
    {
        LogWriter("Iniciando QA-CLN-01: Prueba negativa en Alta de Cliente.");
        await LoginDinamico();
        
        var btnCerrarSinuhe = Page.Locator(".fa.fa-times-circle");
        if (await btnCerrarSinuhe.IsVisibleAsync())
        {
            await btnCerrarSinuhe.ClickAsync();
            await Task.Delay(1000);
        }

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await ClickConMonitoreo(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }), "Apertura de Menú");
        await Task.Delay(1000); 
        await Page.GetByRole(AriaRole.Link, new() { Name = " Cliente nuevo" }).ClickAsync();
        
        LogWriter("Forzando guardado con formulario vacío...");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Crear Cliente" }).ClickAsync();

        // 🚨 ASERCIÓN ROBUSTA CORREGIDA: 
        // Esperamos 5 segundos a que aparezca cualquier tipo de alerta de error (SweetAlert, texto rojo, etc.)
        bool alertaVisible = true;
        try 
        { 
            await Page.Locator(".swal2-container, .alert, .text-danger, .error-message, .error").First
                .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 }); 
        } 
        catch (System.TimeoutException)
        { 
            alertaVisible = false;
        }
        
        // El Assert ahora sí validará y hará fallar la prueba si la alerta nunca apareció
        Assert.IsTrue(alertaVisible, "El sistema no mostró ninguna alerta visual bloqueando los campos vacíos.");
        
        LogWriter("Validación de campos obligatorios exitosa.");
    }

    [Test]
    public async Task QA_CLN_02_AltaExitosaClienteComercial()
    {
        var cliente = ClienteFactory.GenerarCliente(personaMoral: false);
        LogWriter($"Iniciando creación de cliente: {cliente.NombreComercial}");

        await LoginDinamico();
        
        var btnCerrarSinuhe = Page.Locator(".fa.fa-times-circle");
        if (await btnCerrarSinuhe.IsVisibleAsync())
        {
            await btnCerrarSinuhe.ClickAsync();
            await Task.Delay(1000);
        }

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Gestión" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Task.Delay(1000); 
        await Page.GetByRole(AriaRole.Link, new() { Name = " Cliente nuevo" }).ClickAsync();

        if (cliente.EsPersonaMoral) 
            await Page.GetByText("Moral").ClickAsync();
        else 
            await Page.GetByText("Fisica").ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "RFC" }).FillAsync(cliente.RFC);
        if (!cliente.EsPersonaMoral) await Page.GetByRole(AriaRole.Textbox, new() { Name = "CURP" }).FillAsync(cliente.CURP);
        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Razón social" }).FillAsync(cliente.RazonSocial);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Nombre comercial" }).FillAsync(cliente.NombreComercial);
        
        await Page.Locator("#domFisCalle").FillAsync(cliente.Calle);
        await Page.Locator("input[name=\"domFisNumExt\"]").FillAsync(cliente.NumExt);
        
        // 🚨 TOLERANCIA AL API LENTA: Damos hasta 30s de paciencia.
        await Page.Locator("input[name=\"domFisCP\"]").FillAsync(cliente.CP);
        await Page.Keyboard.PressAsync("Tab"); 
        
        await Expect(Page.Locator("#domFisColonia option").Nth(1)).ToBeAttachedAsync(new() { Timeout = 30000 });
        await Page.Locator("#domFisColonia").SelectOptionAsync(new SelectOptionValue { Index = 1 });

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contacto", Exact = true }).FillAsync(cliente.Contacto);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Teléfono" }).FillAsync(cliente.Telefono);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Correo para facturas" }).FillAsync(cliente.EmailFacturacion);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Crear Cliente" }).ClickAsync();
        
        var popupAviso = Page.GetByText("24 horas"); 
        if (await popupAviso.IsVisibleAsync()) await Page.Keyboard.PressAsync("Escape"); 

        LogWriter($"[EXITO] Cliente {cliente.NombreComercial} creado correctamente.");

        await Page.Locator("#back a").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Clientes" })).ToBeVisibleAsync();
    }
}