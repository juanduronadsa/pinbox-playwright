using System;
using System.IO;
using System.Text.Json;
using Bogus;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

/// <summary>
/// Generador de datos (Data Builder Pattern) para el formulario de Nuevo Cliente.
/// Inyecta datos dinámicos y fiscalmente válidos utilizando la librería Bogus,
/// optimizando las lecturas a disco mediante caché estática.
/// </summary>
public static class ClienteFactory
{
    // Caché en memoria para evitar lecturas repetitivas al disco duro (Optimización I/O)
    private static readonly string _rfcFijo;
    private static readonly string _rfcMoralFijo;

    static ClienteFactory()
    {
        string rutaJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "ClienteFijo.json");
        if (!File.Exists(rutaJson)) throw new FileNotFoundException($"[FATAL] No se encontró la configuración fiscal en: {rutaJson}");
        
        var jsonContent = File.ReadAllText(rutaJson);
        using JsonDocument doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;
        
        _rfcFijo = root.GetProperty("RfcFijo").GetString()!;
        _rfcMoralFijo = root.GetProperty("RfcMoralFijo").GetString()!;
    }

    /// <summary>
    /// Retorna un modelo de cliente listo para inyectarse en los tests de Playwright.
    /// </summary>
    /// <param name="personaMoral">Bandera para alternar entre flujos fiscales (Física vs Moral).</param>
    public static ClienteModel GenerarCliente(bool personaMoral = false)
    {
        var f = new Faker("es_MX");
        var direccion = ObtenerDireccionAleatoriaCDMX(f);

        return new ClienteModel
        {
            EsPersonaMoral = personaMoral,
            
            // --- DATOS FISCALES (Extraídos de la caché estática) ---
            RFC = personaMoral ? _rfcMoralFijo : _rfcFijo,
            CURP = personaMoral ? string.Empty : $"{f.Random.AlphaNumeric(18).ToUpper()}",
            
            // --- DATOS GENERALES ---
            Nombre = personaMoral ? $"EMPRESA QA {f.Random.Number(100,999)}" : f.Name.FirstName(),
            RazonSocial = personaMoral ? $"SA DE CV QA {f.Random.Number(100,999)}" : "FISICA TEST",

            // --- DIRECCIÓN (Restringido a CDMX para match con SEPOMEX) ---
            Estado = "Distrito Federal",
            Ciudad = direccion.Delegacion,
            Calle = direccion.Calle,
            NumExt = direccion.Num,
            CP = direccion.CP,
            ColoniaValue = direccion.ColoniaVal
        };
    }

    /// <summary>
    /// Devuelve una dirección validada de la CDMX de forma aleatoria para stress-testing.
    /// </summary>
    private static (string Delegacion, string Calle, string Num, string CP, string ColoniaVal) ObtenerDireccionAleatoriaCDMX(Faker f)
    {
        var poolDirecciones = new[]
        {
            ("Venustiano Carranza", "Africa", f.Address.BuildingNumber(), "15400", "14783|ROMERO RUBIO"),
            ("Cuauhtémoc", "Paseo de la Reforma", f.Address.BuildingNumber(), "06600", "1|JUAREZ"), 
            ("Benito Juárez", "Av. Universidad", f.Address.BuildingNumber(), "03100", "15418|DEL VALLE CENTRO")
        };
        return f.PickRandom(poolDirecciones);
    }
}