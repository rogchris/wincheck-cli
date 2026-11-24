using System.Text;
using System.Text.Json;
using WinCheck;

Console.OutputEncoding = Encoding.UTF8;
string configPath = "settings.json";
string outputPath = "reports/latest.json";
bool demo = false;

try
{
    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--demo":
                demo = true;
                break;
            case "--config" when i + 1 < args.Length:
                configPath = args[++i];
                break;
            case "--output" when i + 1 < args.Length:
                outputPath = args[++i];
                break;
            case "--help":
                Console.WriteLine("WinCheck [--demo] [--config DATEI] [--output DATEI]");
                return 0;
            default:
                throw new ArgumentException($"Unbekannte Option oder fehlender Wert: {args[i]}");
        }
    }

    if (!demo && !OperatingSystem.IsWindows())
    {
        Console.Error.WriteLine("Die Systemprüfung benötigt Windows. Mit --demo kannst du die Ausgabe ansehen.");
        return 2;
    }

    var settings = Settings.Load(configPath);
    List<CheckResult> results;
    if (demo)
    {
        // Beispieldaten, damit sich die Ausgabe auch ohne Windows ansehen lässt.
        results =
        [
            SystemChecks.EvaluateDisk(@"C:\", 500L * 1024 * 1024 * 1024, 100L * 1024 * 1024 * 1024, settings.MinimumFreePercent),
            SystemChecks.EvaluateDisk(@"D:\", 100L * 1024 * 1024 * 1024, 5L * 1024 * 1024 * 1024, settings.MinimumFreePercent)
        ];
        foreach (string name in settings.Services.Distinct(StringComparer.OrdinalIgnoreCase))
            results.Add(new CheckResult(name, "OK", "Status: Running (Beispiel)"));
    }
    else if (OperatingSystem.IsWindows())
    {
        results = SystemChecks.CheckDisks(settings.MinimumFreePercent);
        results.AddRange(SystemChecks.CheckServices(settings.Services));
    }
    else
    {
        return 2;
    }

    var report = new CheckReport(DateTimeOffset.Now, demo ? "DEMO-PC" : Environment.MachineName, results);
    Console.WriteLine(demo ? "WinCheck – DEMO mit Beispieldaten" : $"WinCheck – {report.Computer}");
    foreach (var result in results)
        Console.WriteLine($"[{result.Status}] {result.Name}: {result.Detail}");

    string fullOutputPath = Path.GetFullPath(outputPath);
    string fullConfigPath = Path.GetFullPath(configPath);
    if (string.Equals(fullOutputPath, fullConfigPath, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Bericht und Konfiguration müssen verschiedene Dateien sein.");

    Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);
    var jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    File.WriteAllText(fullOutputPath, JsonSerializer.Serialize(report, jsonOptions));
    Console.WriteLine($"Bericht: {fullOutputPath}");
    return report.ExitCode;
}
catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or ArgumentException)
{
    Console.Error.WriteLine($"Fehler: {exception.Message}");
    return 2;
}
