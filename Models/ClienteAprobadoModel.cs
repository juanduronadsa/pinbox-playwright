using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: CLIENTE DEL DATA POOL (REGLA 24 HRS)
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/QA_GST_ClientesAprobados.json
// Consumido por: Factories/ClientePoolFactory.cs
// ---------------------------------------------------------
/// <summary>
/// Representa una entidad cliente pre-aprobada que ha superado el ciclo de sincronización del negocio.
/// Mapeo inmutable consumido exclusivamente por los vectores de prueba de Cotización E2E.
/// </summary>
public class ClienteAprobadoModel
{
    [JsonPropertyName("nombre")]
    public string Nombre { get; init; } = string.Empty;
    
    [JsonPropertyName("idCliente")]
    public string IdCliente { get; init; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string Telefono { get; init; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; init; } = string.Empty;

    [JsonPropertyName("estado")]
    public string Estado { get; init; } = string.Empty;

    [JsonPropertyName("municipio")]
    public string Municipio { get; init; } = string.Empty;

    [JsonPropertyName("categoria")]
    public string Categoria { get; init; } = string.Empty;
    
    [JsonPropertyName("usuarioPropietario")]
    public string UsuarioPropietario { get; init; } = string.Empty;

    [JsonPropertyName("passwordPropietario")]
    public string PasswordPropietario { get; init; } = string.Empty;
}