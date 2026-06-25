using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA AUTENTICACIÓN E IDENTIDAD
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_LGN_Data.json
// Consumido por: Tests de Autenticación y Dashboard
// ---------------------------------------------------------
/// <summary>
/// Estructura para inyectar flujos de login (caminos felices y alternos).
/// Verifica que las credenciales respondan con el mensaje de error adecuado 
/// o que el nombre del Agente en el Dashboard coincida con la cuenta ingresada.
/// </summary>
public class UsuarioTestData
{
    [JsonPropertyName("casoId")]
    public string CasoId { get; init; } = string.Empty;

    [JsonPropertyName("usuario")]
    public string Usuario { get; init; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;

    [JsonPropertyName("debeSerExitoso")]
    public bool DebeSerExitoso { get; init; }

    [JsonPropertyName("mensajeEsperado")]
    public string MensajeEsperado { get; init; } = string.Empty;

    [JsonPropertyName("agenteEsperado")]
    public string AgenteEsperado { get; init; } = string.Empty;
}