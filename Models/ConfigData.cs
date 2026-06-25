using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: CONFIGURACIÓN GLOBAL DEL AMBIENTE
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/Global_Credentials.json
// Consumido por: Core/BaseTest.cs (Método SetupGlobal)
// ---------------------------------------------------------
/// <summary>
/// Mapea de forma inmutable las credenciales de acceso base y la URL del entorno bajo prueba (QA/UAT).
/// Actúa como el contrato de infraestructura inicial inyectado globalmente al arrancar el motor.
/// </summary>
public class ConfigData
{
    [JsonPropertyName("user")]
    public string User { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("agenteEsperado")]
    public string AgenteEsperado { get; set; } = string.Empty;
}