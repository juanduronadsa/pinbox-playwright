using System.Text.Json.Serialization;
namespace Playwrigt_Demo.Models;

// ---------------------------------------------------------
// MODELO: ENTIDAD CLIENTE NUEVO (CREACIÓN)
// ---------------------------------------------------------
// 🔗 VINCULACIÓN DE ARCHIVOS:
// Archivo JSON: TestData/ClienteFijo.json (Para datos seguros)
// Consumido por: Factories/ClienteFactory.cs y Tests de Alta
// ---------------------------------------------------------
/// <summary>
/// Estructura operativa utilizada para la persistencia transitoria en el formulario de "Nuevo Cliente".
/// Diseñada con mutabilidad explícita para la inyección dinámica de datos a través de generadores semánticos.
/// </summary>
public class ClienteModel
{
    public bool EsPersonaMoral { get; set; }

    // --- Identificadores Fiscales ---
    public string RFC { get; set; } = string.Empty;
    public string CURP { get; set; } = string.Empty;

    // --- Identidades Comerciales ---
    public string Nombre { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;

    // --- Canales de Comunicación ---
    public string Telefono { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string EmailFacturacion { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;

    // --- Desglose Geográfico (Estructura SEPOMEX) ---
    public string Estado { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumExt { get; set; } = string.Empty;
    public string NumInt { get; set; } = string.Empty;
    public string CP { get; set; } = string.Empty;
    public string ColoniaValue { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
}