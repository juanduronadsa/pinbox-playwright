using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA COTIZADOR MÚLTIPLE
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_GST_CotizacionData.json
// Consumido por: Tests/QA_GST_CotizacionTests.cs
// ---------------------------------------------------------
/// <summary>
/// Estructura que define el caso de prueba para vender un producto específico del catálogo.
/// </summary>
public class CotizacionTestData
{
    [JsonPropertyName("casoId")]
    public string CasoId { get; init; } = string.Empty;

    [JsonPropertyName("producto")]
    public string Producto { get; init; } = string.Empty;

    [JsonPropertyName("periodo")]
    public string Periodo { get; init; } = string.Empty;

    [JsonPropertyName("suscripcion")]
    public string Suscripcion { get; init; } = string.Empty;

    [JsonPropertyName("metodoPago")]
    public string MetodoPago { get; init; } = string.Empty;

    [JsonPropertyName("leyendas")]
    public List<string> Leyendas { get; init; } = new();
}