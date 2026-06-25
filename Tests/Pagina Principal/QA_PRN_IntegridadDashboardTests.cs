using System;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playwrigt_Demo;

[TestFixture]
[Category("Dashboard")]
[Category("Smoke")]      
[Category("Alta")]       
public class QA_PRN_IntegridadDashboardTests : BaseTest 
{
    [SetUp]
    public async Task SetupDashboard()
    {
        // Esto iniciará sesión automáticamente con "sinuhe.romo"
        await LoginDinamico();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync();
    }

    // 🚨 TEST REFACTORIZADO Y SIMPLIFICADO
    [Test]
    public async Task QA_PRN_14_ValidarIdentidadEnDashboard()
    {
        // Usamos directamente Config.AgenteEsperado que viene de tu JSON Global
        LogWriter($"Validando la identidad del usuario en sesión. Buscando: {Config.AgenteEsperado}");
        
        // Buscamos el texto exacto agnóstico al HTML y esperamos a que el DOM lo dibuje
        var elementoUsuario = Page.GetByText(Config.AgenteEsperado).First;
        
        // Le damos hasta 15 segundos al servidor para que traiga el nombre y lo pinte
        await elementoUsuario.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 15000 });
        
        // Aserción final
        await Expect(elementoUsuario).ToBeVisibleAsync();
    }

    [Test]
    public async Task QA_PRN_16_ValidarTarjetasDeEstado()
    {
        LogWriter("Iniciando validación determinista de tarjetas de estado interactivo.");

        var tarjetas = new Dictionary<string, string>
        {
            { "#btnComercialNo", "No Elaborado" },
            { "#btnComercialPendiente", "Pendiente" }
        };

        foreach (var tarjeta in tarjetas)
        {
            LogWriter($"Despachando evento Click sobre tarjeta: {tarjeta.Value}");
            await ClickConMonitoreo(Page.Locator(tarjeta.Key), $"Tarjeta {tarjeta.Value}");
            
            await Expect(Page.Locator("#pptoComercial")).ToBeVisibleAsync();
            
            await Page.Keyboard.PressAsync("Escape");
            await Page.Locator(".modal-sn").WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        }
        
        LogWriter("Ciclo de tarjetas de estado completado sin bloqueos de interfaz.");
    }
}