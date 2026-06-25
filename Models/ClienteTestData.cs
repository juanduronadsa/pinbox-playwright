using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: DATA-DRIVEN PARA ALTA DE CLIENTES
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_GST_ClienteData.json
// Consumido por: Tests de Alta de Cliente
// ---------------------------------------------------------
/// <summary>
/// Define las iteraciones y reglas de negocio con las que se ejecutará la prueba de creación de clientes.
/// </summary>
public class ClienteTestData
{
    [JsonPropertyName("casoId")]
    public string CasoId { get; init; } = string.Empty;

    [JsonPropertyName("tipoPersona")]
    public string TipoPersona { get; init; } = string.Empty;

    [JsonPropertyName("categoriaValor")]
    public string CategoriaValor { get; init; } = string.Empty;

    [JsonPropertyName("rfc")]
    public string RFC { get; init; } = string.Empty; 

    [JsonPropertyName("curp")]
    public string CURP { get; init; } = string.Empty; 
}