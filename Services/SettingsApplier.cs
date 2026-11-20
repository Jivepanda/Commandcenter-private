using System.Diagnostics;

namespace CommandCentre.Services;

public static class SettingsApplier
{
    static string SchemeFor(string plan) => plan.ToLowerInvariant() switch
    {
        "performance" => "SCHEME_MIN",
        "saver" => "SCHEME_MAX",
        _ => "SCHEME_BALANCED"
    };

    // Returns false if this PC doesn't have that plan
    public static bool SetPowerPlan(string plan)
    {
        var psi = new ProcessStartInfo("powercfg", $"/setactive {SchemeFor(plan)}")
        {
            CreateNoWindow = true,
            UseShellExecute = false
        };
        using var p = Process.Start(psi)!;
        p.WaitForExit();
        return p.ExitCode == 0;
    }
}