using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Playwrigt_Demo.Models;

namespace Playwrigt_Demo.Factories;

// ---------------------------------------------------------
// FACTORY: POOL DE CLIENTES (ROUND-ROBIN) THREAD-SAFE
// ---------------------------------------------------------
/// <summary>
/// Administra una colección de clientes pre-aprobados.
/// Implementa un algoritmo de Turno Circular (Round-Robin) seguro para hilos (Thread-Safe),
/// preparado para distribuir equitativamente la carga en futuras ejecuciones en paralelo.
/// </summary>
public static class ClientePoolFactory
{
    private static readonly List<ClienteAprobadoModel> _pool = new();
    private static int _indiceActual = 0; 
    
    // Objeto de bloqueo para garantizar seguridad en concurrencia (Paralelismo)
    private static readonly object _lock = new object();

    static ClientePoolFactory()
    {
        string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "QA_GST_ClientesAprobados.json");
        
        if (File.Exists(ruta))
        {
            var jsonContent = File.ReadAllText(ruta);
            _pool = JsonSerializer.Deserialize<List<ClienteAprobadoModel>>(jsonContent) ?? new();
        }
        else
        {
            throw new FileNotFoundException($"[FATAL] No se encontró el pool de clientes en: {ruta}");
        }
    }

    /// <summary>
    /// Devuelve el cliente correspondiente al turno actual garantizando seguridad de memoria concurrente.
    /// </summary>
    public static ClienteAprobadoModel ObtenerClienteEnTurno()
    {
        if (!_pool.Any()) throw new InvalidOperationException("El pool de clientes está vacío. Verifica QA_GST_ClientesAprobados.json");

        lock (_lock) // 🚨 Bloqueo asíncrono para evitar colisiones si se activa el paralelismo
        {
            var cliente = _pool[_indiceActual];
            _indiceActual = (_indiceActual + 1) % _pool.Count;
            return cliente;
        }
    }

    /// <summary>
    /// Devuelve un cliente aleatorio para pruebas exploratorias o negativas.
    /// </summary>
    public static ClienteAprobadoModel ObtenerClienteAleatorio()
    {
        if (!_pool.Any()) throw new InvalidOperationException("El pool de clientes está vacío.");
        
        var rnd = new Random();
        return _pool[rnd.Next(_pool.Count)];
    }
}