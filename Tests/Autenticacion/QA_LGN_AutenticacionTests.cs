using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo;

// ---------------------------------------------------------
// SUITE DE PRUEBAS: AUTENTICACIÓN E IDENTIDAD
// ---------------------------------------------------------
/// <summary>
/// Contiene las pruebas de validación de acceso al sistema (Login).
/// Utiliza una arquitectura "Data-Driven" para certificar múltiples vectores de ataque y accesos legítimos.
/// </summary>
[TestFixture]
[Category("Auth")]       
[Category("Smoke")]      
[Category("Critica")]    
public class QA_LGN_AutenticacionTests : BaseTest 
{
    /// <summary>
    /// Proveedor de Datos Estricto (Data Provider).
    /// Mapea los vectores de prueba hacia el modelo inmutable UsuarioTestData.
    /// </summary>
    public static IEnumerable<TestCaseData> LeerDatos()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_LGN_Data.json");
        var json = File.ReadAllText(rutaJson);
        
        // Deserialización tipada basada en los contratos (Models)
        var usuarios = JsonSerializer.Deserialize<List<UsuarioTestData>>(json)!;

        foreach (var usuario in usuarios)
        {
            var testCase = new TestCaseData(usuario)
                .SetName($"{usuario.CasoId}_ValidarFlujoDeLogin"); 

            // Documentación viva de deuda técnica o bugs conocidos
            if (usuario.CasoId == "QA-LGN-02")
            {
                testCase.Ignore("Bug Crítico Reportado: El ambiente permite inicio de sesión con contraseñas falsas (Ticket #1234).");
            }

            yield return testCase;
        }
    }

    /// <summary>
    /// Flujo atómico de validación de credenciales. Asegura que el motor de identidad responda correctamente.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(LeerDatos))]
    public async Task QA_LGN_ValidarFlujoDeLogin(UsuarioTestData datos)
    {
        LogWriter($"Inyectando vector de prueba de autenticación: {datos.Usuario}");

        // 1. Inyección de datos
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(datos.Usuario);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(datos.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();

        // 2. Aserciones Deterministas basadas en el contrato de datos
        if (datos.DebeSerExitoso)
        {
            // HAPPY PATH: Validamos resolución del DOM post-login
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" })).ToBeVisibleAsync(new() { Timeout = 15000 });
            await Expect(Page.Locator("body")).ToContainTextAsync(datos.AgenteEsperado);
            LogWriter("Autenticación exitosa. Identidad del agente validada en el DOM.");
        }
        else
        {
            // NEGATIVE TESTING: Validamos intercepción de errores de seguridad
            await Expect(Page.GetByText(datos.MensajeEsperado)).ToBeVisibleAsync();
            LogWriter($"Autenticación denegada correctamente. Razón validada: {datos.MensajeEsperado}");
        }
    }
}