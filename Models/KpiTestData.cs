using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA MÉTRICAS (KPIs)
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_PRN_KpisData.json
// Consumido por: Tests/QA_PRN_NavegacionKpisTests.cs
// ---------------------------------------------------------
/// <summary>
/// Estructura para iterar sobre los botones circulares del Dashboard (KPIs) 
/// y validar que la navegación hacia sus detalles redirija correctamente.
/// </summary>
public class KpiTestData
{
    [JsonPropertyName("casoId")]
    public string CasoId { get; init; } = string.Empty;

    [JsonPropertyName("boton")]
    public string Boton { get; init; } = string.Empty;

    [JsonPropertyName("tituloEsperado")]
    public string TituloEsperado { get; init; } = string.Empty;

    [JsonPropertyName("esId")]
    public bool EsId { get; init; }
}