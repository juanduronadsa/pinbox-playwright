using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

/// <summary>
/// Motor de reglas de negocio para el Catálogo de Productos.
/// Extrae la configuración estática validando tolerancia a fallos y mapeo flexible.
/// </summary>
public static class ProductoFactory
{
    private static List<ProductoCatalogoModel> _catalogo = new();

    static ProductoFactory()
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "CatalogoMaestro.json");

        if (File.Exists(ruta))
        {
            var jsonContent = File.ReadAllText(ruta);
            
            // 🚨 FIX 1: Obliga a C# a ignorar si el JSON viene en mayúsculas o minúsculas
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _catalogo = JsonSerializer.Deserialize<List<ProductoCatalogoModel>>(jsonContent, opciones) ?? new();
        }
        else
        {
            throw new FileNotFoundException($"[FATAL] No se encontró el catálogo maestro en: {ruta}");
        }
    }

    /// <summary>
    /// Busca un producto en el catálogo de forma segura.
    /// </summary>
    public static ProductoCatalogoModel? ObtenerConfiguracion(string nombreProducto)
    {
        // 🚨 FIX 2: Cortafuegos. Si por algún motivo el nombre llega nulo, lo rebotamos antes de que rompa el test.
        if (string.IsNullOrEmpty(nombreProducto)) return null;

        // 🚨 FIX 3: String.Equals con OrdinalIgnoreCase previene el NullReferenceException 
        // a diferencia de usar .ToLower() o ==.
        return _catalogo.FirstOrDefault(p => 
            string.Equals(p.Producto, nombreProducto, StringComparison.OrdinalIgnoreCase));
    }
}