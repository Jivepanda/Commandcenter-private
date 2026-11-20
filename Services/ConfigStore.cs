using System.IO;
using System.Text.Json;
using CommandCentre.Models;

namespace CommandCentre.Services;

public static class ConfigStore
{
    static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static Config Load()
    {
        var file = Path.Combine(AppContext.BaseDirectory, "modes.json");
        var json = File.ReadAllText(file);
        return JsonSerializer.Deserialize<Config>(json, Options) ?? new Config();
    }
}