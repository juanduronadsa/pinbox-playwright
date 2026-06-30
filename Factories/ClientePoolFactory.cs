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
    /// Devuelve uno de los agentes ÚNICOS del pool (sin repetir, deduplicado por usuario) de forma
    /// determinista según el nombre del caso. Pensado para módulos como "Alta de Clientes" que no
    /// necesitan un cliente aprobado específico, solo distribuir la carga entre varias cuentas
    /// reales en vez de saturar siempre a la misma (ej. Config.User).
    /// </summary>
    public static ClienteAprobadoModel ObtenerAgentePorCaso(string nombreCaso)
    {
        if (!_pool.Any()) throw new InvalidOperationException("El pool de clientes está vacío. Verifica QA_GST_ClientesAprobados.json");

        var agentesUnicos = _pool
            .GroupBy(c => c.UsuarioPropietario)
            .Select(g => g.First())
            .OrderBy(c => c.UsuarioPropietario) // orden estable, no depende del orden del JSON
            .ToList();

        if (string.IsNullOrEmpty(nombreCaso)) return agentesUnicos[0];

        unchecked
        {
            int hash = 17;
            foreach (char c in nombreCaso) hash = hash * 31 + c;
            int indice = Math.Abs(hash) % agentesUnicos.Count;
            return agentesUnicos[indice];
        }
    }

    /// <summary>
    /// Devuelve siempre el MISMO cliente para el mismo CasoId, sin importar el orden ni si se
    /// ejecuta la suite completa o un único caso aislado (ej. al reintentar un test que falló).
    /// A diferencia de ObtenerClienteEnTurno(), esto es 100% reproducible entre corridas.
    /// </summary>
    public static ClienteAprobadoModel ObtenerClientePorCaso(string casoId)
    {
        if (!_pool.Any()) throw new InvalidOperationException("El pool de clientes está vacío. Verifica QA_GST_ClientesAprobados.json");
        if (string.IsNullOrEmpty(casoId)) return ObtenerClienteEnTurno();

        // Hash estable (no usamos string.GetHashCode() porque varía entre ejecuciones de .NET)
        unchecked
        {
            int hash = 17;
            foreach (char c in casoId) hash = hash * 31 + c;
            int indice = Math.Abs(hash) % _pool.Count;
            return _pool[indice];
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