using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Text.Json; 
using System.Threading.Tasks;
using Playwrigt_Demo.Models; 

// ---------------------------------------------------------
// CONFIGURACIÓN DE RENDIMIENTO (HARDWARE)
// ---------------------------------------------------------
[assembly: Parallelizable(ParallelScope.Fixtures)] 
[assembly: LevelOfParallelism(1)] 

namespace Playwrigt_Demo;

/// <summary>
/// Motor principal de configuración global.
/// Se ejecuta UNA SOLA VEZ antes de que comience toda la suite de pruebas.
/// </summary>
[SetUpFixture]
public class GlobalSetup
{
    /// <summary>
    /// Gestiona la limpieza inteligente de evidencias. Solo elimina registros previos
    /// si se detecta explícitamente la bandera de ejecución completa de la suite.
    /// </summary>
    [OneTimeSetUp]
    public void LimpiarDirectoriosDeEvidencia()
    {
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        string[] directoriosReportes = { 
            Path.Combine(baseDir, "Reportes", "Videos"), 
            Path.Combine(baseDir, "Reportes", "Traces"),
            Path.Combine(baseDir, "Reportes", "Network"),
            Path.Combine(baseDir, "Reportes", "Logs")
        };

        // 🚨 CONTROL DE LIMPIEZA INTELIGENTE:
        // Solo eliminamos la evidencia vieja si se activa el flag CLEAR_ALL="1" desde la consola.
        if (Environment.GetEnvironmentVariable("CLEAR_ALL") == "1")
        {
            Console.WriteLine("[GLOBAL SETUP] Ejecución total detectada. Limpiando todo el historial de evidencias...");
            foreach (var directorio in directoriosReportes)
            {
                if (Directory.Exists(directorio))
                {
                    try { Directory.Delete(directorio, true); } catch { } 
                }
                Directory.CreateDirectory(directorio);
            }
        }
        else
        {
            Console.WriteLine("[GLOBAL SETUP] Ejecución modular/parcial detectada. Preservando el historial de otros módulos...");
            // Aseguramos que existan las carpetas raíz por si es la primera corrida en limpio
            foreach (var directorio in directoriosReportes)
            {
                if (!Directory.Exists(directorio)) Directory.CreateDirectory(directorio);
            }
        }
    }
}

/// <summary>
/// Clase base de la que DEBEN heredar todas las clases de pruebas (Tests) del framework.
/// </summary>
public class BaseTest : PageTest
{
    protected bool ModoAuditoriaRed = false;
    // 🚨 Locators de las 3 alertas conocidas de "API lenta de terceros" (Sección Amarilla).
    // Expuestos como propiedades para que pruebas específicas puedan desactivar SOLO la que
    // necesitan inspeccionar (con Page.RemoveLocatorHandlerAsync) sin apagar las otras dos.
    protected ILocator AlertaDashboardApiLenta => Page.Locator(".swal2-container").Filter(new() { HasText = "seguir navegando en Pinbox" });
    protected ILocator AlertaSinInformacion => Page.Locator(".swal2-container").Filter(new() { HasText = "no contiene información" });
    protected ILocator AlertaCotizadorVacio => Page.Locator(".swal2-container").Filter(new() { HasText = "No se encontraron resultados" });
    
    private const int TIMEOUT_LIMITE = 45000;
    protected ConfigData Config;

    /// <summary>
    /// Identifica el módulo correspondiente inspeccionando el prefijo del nombre de la prueba.
    /// </summary>
    private string ExtraerModulo(string nombreTest)
    {
        if (string.IsNullOrEmpty(nombreTest)) return "Generales";
        string upper = nombreTest.ToUpper();
        if (upper.Contains("LGN")) return "Login";
        if (upper.Contains("SMK")) return "Smoke";
        if (upper.Contains("PRN")) return "PantallasPrincipales";
        if (upper.Contains("CLN")) return "Clientes";
        if (upper.Contains("GST") || upper.Contains("COT")) return "Cotizaciones";
        return "Generales";
    }

    private string LimpiarNombreTest(string nombreOriginal)
    {
        string limpio = nombreOriginal.Replace(" ", "_").Replace("\"", "").Replace("(", "").Replace(")", "").Replace(",", "");
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            limpio = limpio.Replace(c.ToString(), "");
        }
        return limpio;
    }

    /// <summary>
    /// Configura el entorno de Playwright segmentando dinámicamente las salidas en subcarpetas por módulo.
    /// </summary>
    public override BrowserNewContextOptions ContextOptions() 
    {
        string nombreActual = TestContext.CurrentContext.Test.Name ?? "Desconocido";
        string nombreTest = LimpiarNombreTest(nombreActual);
        string modulo = ExtraerModulo(nombreTest);
        
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        // Obligamos a que la carpeta Network exista antes de grabar el HAR
        string dirNetwork = Path.Combine(baseDir, "Reportes", "Network", modulo);
        if (!Directory.Exists(dirNetwork)) Directory.CreateDirectory(dirNetwork);

        return new() 
        { 
            // 🚨 El video nace en una carpeta temporal segura para que Windows no lo bloquee
            RecordVideoDir = Path.Combine(baseDir, "Reportes", "Videos"),
            RecordHarPath = Path.Combine(dirNetwork, $"{nombreTest}_trafico.har"),
            RecordHarOmitContent = false, 
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 } 
        };
    }

    [SetUp]
    public async Task SetupGlobal()
    {
        LogWriter($"--- INICIANDO PRUEBA: {TestContext.CurrentContext.Test.Name} ---");

        string rutaConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Global_Credentials.json");
        string jsonText = File.ReadAllText(rutaConfig);
        Config = JsonSerializer.Deserialize<ConfigData>(jsonText)!;

        Context.SetDefaultTimeout(TIMEOUT_LIMITE);
        Context.SetDefaultNavigationTimeout(TIMEOUT_LIMITE);
        Assertions.SetDefaultExpectTimeout(TIMEOUT_LIMITE);

        await Context.ClearCookiesAsync();
        await Page.GotoAsync(Config.Url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Page.EvaluateAsync("() => { localStorage.clear(); sessionStorage.clear(); }");
        await Page.ReloadAsync();

        await Context.Tracing.StartAsync(new()
        {
            Title = TestContext.CurrentContext.Test.Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true 
        });

        await Page.AddLocatorHandlerAsync(
            AlertaDashboardApiLenta,
            async () => {
                LogWriter("[ALERTA] API Lenta. Dando clic real en Continuar navegando...");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Continuar navegando." }).ClickAsync();
            }
        );

        await Page.AddLocatorHandlerAsync(
            AlertaSinInformacion,
            async () => {
                LogWriter("[ALERTA] Cliente sin información. Dando clic real en Aceptar...");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Aceptar" }).ClickAsync();
            }
        );
        // 🚨 Guardaespaldas para el popup del Cotizador
        await Page.AddLocatorHandlerAsync(
            AlertaCotizadorVacio,
            async () => {
                LogWriter("[ALERTA] Tabla de Cotizador vacía. Dando clic real en OK...");
                var btnOk = Page.GetByRole(AriaRole.Button, new() { Name = "OK" });
                await btnOk.ClickAsync();
                await Page.Locator(".swal2-container").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            }
        );
    }

    [TearDown]
    public async Task RecolectarEvidenciasAlTerminar()
    {
        string nombreTest = LimpiarNombreTest(TestContext.CurrentContext.Test.Name);
        string modulo = ExtraerModulo(nombreTest);
        
        // 🚨 OBLIGAMOS A SALIR DE BIN: Todo se guarda en la raíz del proyecto
        string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        // 1. Aseguramos que la carpeta DESTINO FINAL del módulo exista
        string dirVideoModulo = Path.Combine(baseDir, "Reportes", "Videos", modulo);
        string dirTraceModulo = Path.Combine(baseDir, "Reportes", "Traces", modulo);
        if (!Directory.Exists(dirVideoModulo)) Directory.CreateDirectory(dirVideoModulo);
        if (!Directory.Exists(dirTraceModulo)) Directory.CreateDirectory(dirTraceModulo);

        // 2. Guardamos Trace directamente en su módulo
        try 
        {
            string rutaTrace = Path.Combine(dirTraceModulo, $"{nombreTest}_trace.zip");
            await Context.Tracing.StopAsync(new() { Path = rutaTrace });
        } 
        catch (Exception ex) { LogWriter($"[EVIDENCIA FALLIDA] Trace: {ex.Message}"); }

        // 3. Capturamos la ruta del video temporal ANTES de cerrar el contexto
        string rutaVideoOriginal = string.Empty;
        try { if (Page?.Video != null) rutaVideoOriginal = await Page.Video.PathAsync(); } catch { }

        // 4. Cerramos el navegador (esto le dice a Playwright que deje de grabar)
        try { await Context.CloseAsync(); } catch { }
        
        // 🚨 FRENO DE WINDOWS: Le damos medio segundo al Sistema Operativo para que suelte el archivo
        await Task.Delay(500); 
        
        // 5. Movemos el video desde la carpeta raíz temporal hasta la carpeta del Módulo
        try 
        {
            if (!string.IsNullOrEmpty(rutaVideoOriginal) && File.Exists(rutaVideoOriginal))
            {
                string rutaVideoDeseada = Path.Combine(dirVideoModulo, $"{nombreTest}.webm");
                if (File.Exists(rutaVideoDeseada)) File.Delete(rutaVideoDeseada);
                
                File.Move(rutaVideoOriginal, rutaVideoDeseada);
                LogWriter($"[EVIDENCIA] Video organizado exitosamente en: {modulo} / {nombreTest}.webm");
            }
        } 
        catch (Exception ex) { LogWriter($"[ERROR ARCHIVO] No se pudo mover el video: {ex.Message}"); }

        LogWriter($"--- FINALIZADO: {TestContext.CurrentContext.Test.Name} | ESTADO: {TestContext.CurrentContext.Result.Outcome.Status} ---\n");
    }

    protected async Task LoginDinamico()
    {
        LogWriter("Inyectando credenciales dinámicas...");
    
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(Config.User);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(Config.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 15000 });
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        
        // Nota: el cierre de SweetAlerts inesperados (Dashboard, Sinergias, Cotizador, etc.)
        // ya lo gestionan los AddLocatorHandlerAsync registrados en SetupGlobal. Verificarlos
        // de nuevo aquí "a mano" con .swal2-confirm es peligroso: en modales de 2 botones
        // (ej. "Recargar" / "Continuar navegando.") esa clase no siempre apunta al botón que
        // queremos, y choca con el handler global, dejando la prueba esperando un botón que
        // ya no existe. Verificamos directamente el destino final del login.
        var menuOpenButton = Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });
        await Expect(menuOpenButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15000 });
        LogWriter("Login exitoso y Dashboard verificado.");
    }

    protected async Task LoginEspecifico(string usuario, string password)
    {
        LogWriter($"Inyectando credenciales específicas para el agente: {usuario}");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Usuario" }).FillAsync(usuario);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" }).ClickAsync();
        await Expect(Page.Locator("#divLoading")).ToBeHiddenAsync();
        try { await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 }); } catch { }
    }
    
    /// <summary>
    /// Imprime en consola (para el reporte HTML) y guarda físicamente en un archivo .log
    /// </summary>
    protected void LogWriter(string mensaje)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string lineaLog = $"[{timestamp}] {mensaje}";
        
        // 1. Imprimir en consola normal (NUnit lo atrapa para el HTML)
        Console.WriteLine(lineaLog);

        // 2. Escribir en un archivo .log físico
        try 
        {
            // Obtener el nombre de la prueba y la ruta base (afuera de bin)
            string nombreActual = TestContext.CurrentContext.Test.Name ?? "Ejecucion_Global";
            string nombreTest = LimpiarNombreTest(nombreActual);
            string modulo = ExtraerModulo(nombreTest);
            string baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            
            // Crear la carpeta del módulo dentro de Logs (ej. Reportes/Logs/Smoke)
            string dirLogs = Path.Combine(baseDir, "Reportes", "Logs", modulo);
            if (!Directory.Exists(dirLogs)) Directory.CreateDirectory(dirLogs);

            // Anexar la línea al archivo (AppendAllText crea el archivo si no existe)
            string rutaArchivoLog = Path.Combine(dirLogs, $"{nombreTest}.log");
            File.AppendAllText(rutaArchivoLog, lineaLog + Environment.NewLine);
        }
        catch 
        { 
            // Ignorar errores de I/O silenciosamente para no hacer fallar la prueba
        }
    }

    protected async Task ClickConMonitoreo(ILocator localizador, string nombreAccion)
    {
        // Esperamos hasta 10 segundos a que el elemento sea visible y estable antes de hacer clic.
        await localizador.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

        var cronometro = System.Diagnostics.Stopwatch.StartNew();
        await localizador.ClickAsync();
        cronometro.Stop();
    
        if (cronometro.ElapsedMilliseconds > 15000) 
            LogWriter($"[⚠️ ADVERTENCIA] Latencia detectada en {nombreAccion}: {cronometro.ElapsedMilliseconds}ms.");
    }
}