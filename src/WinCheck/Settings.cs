using System.Text.Json;
using System.Text.Json.Serialization;

namespace WinCheck;

public class Settings
{
    public double MinimumFreePercent { get; set; } = 15;
    public string[] Services { get; set; } = ["EventLog", "LanmanWorkstation"];

    public static Settings Load(string path)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
        var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(path), options)
            ?? throw new InvalidDataException("Die Konfiguration ist leer.");
        settings.Validate();
        return settings;
    }

    public void Validate()
    {
        if (!double.IsFinite(MinimumFreePercent) || MinimumFreePercent <= 0 || MinimumFreePercent >= 100)
            throw new InvalidDataException("MinimumFreePercent muss zwischen 0 und 100 liegen, ohne die Grenzen.");

        if (Services is null || Services.Any(string.IsNullOrWhiteSpace))
            throw new InvalidDataException("Services muss ein Array mit gültigen Dienstnamen sein.");
    }
}
