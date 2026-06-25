using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: CATÁLOGO FUERTEMENTE TIPADO
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_GST_CatalogoProductos.json
// Consumido por: Factories/ProductoFactory.cs
// ---------------------------------------------------------
/// <summary>
/// Representa las especificaciones comerciales de un producto extraídas del catálogo maestro.
/// Determina de forma dinámica si la automatización debe interactuar con flujos de periodos o suscripciones.
/// </summary>
public class ProductoCatalogoModel
{
    [JsonPropertyName("producto")]
    public string Producto { get; init; } = string.Empty;

    [JsonPropertyName("codigo")]
    public string Codigo { get; init; } = string.Empty;

    [JsonPropertyName("periodos_validos")]
    public List<string> PeriodosValidos { get; init; } = new();

    [JsonPropertyName("suscripciones_validas")]
    public List<string> SuscripcionesValidas { get; init; } = new();
}