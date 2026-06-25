using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA ENLACES Y MENÚS
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivos JSON: QA_SMK_TableroLinksData.json y QA_SMK_AyudasLinksData.json
// Consumido por: Tests del Menú Lateral (Dashboard y Ayudas)
// ---------------------------------------------------------
/// <summary>
/// Modelo utilizado para hacer pruebas de "Smoke" sobre la navegación del menú izquierdo.
/// Valida si un enlace está roto, deshabilitado o si abre una pestaña externa (Target Blank).
/// </summary>
public class LinkTestData
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("textoEnlace")]
    public string TextoEnlace { get; init; } = string.Empty;

    [JsonPropertyName("selectorValidacion")]
    public string SelectorValidacion { get; init; } = string.Empty;

    [JsonPropertyName("habilitado")]
    public bool Habilitado { get; init; }

    [JsonPropertyName("esExterno")]
    public bool EsExterno { get; init; }
}