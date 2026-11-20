using CommandCentre.Models;

namespace CommandCentre.Services;

public class ModeManager(Config config, Action<string> log)
{
    //Runing apps from other modes that the next mode dosn't use
    public List<AppEntry> AppsToClose(Mode next) => config.Modes
        .Where(m => m != next)
        .SelectMany(m => m.Apps)
        .Where(a => !next.Apps.Any(n => SameApp(n, a)))
        .Where(a => !config.KeepAlways.Contains(AppController.ProcessNameOf(a), StringComparer.OrdinalIgnoreCase))
        .Where(AppController.IsRunning)
        .DistinctBy(AppController.ProcessNameOf)
        .ToList();

    public List<AppEntry> AppsToOpen(Mode next) => next.Apps
        .Where(a => !AppController.IsRunning(a))
        .ToList();

    public async Task SwitchTo(Mode next)
    {
        log($"Switching to {next.Name}");

        var closing = AppsToClose(next);
        foreach (var app in closing)
        {
            log($"  Asking to close {app.Name}");
            AppController.AskToClose(app);
        }

        if (closing.Count > 0)
        {
            await Task.Delay(5000); //wait for apps to close
            foreach (var app in closing.Where(AppController.IsRunning))
                log($"  ! {app.Name} is still open (it may be waiting for you to save)");
        }

        var ok = SettingsApplier.SetPowerPlan(next.PowerPlan);
        log(ok ? $"  Power plan set to {next.PowerPlan}"
               : $"  ! Power plan {next.PowerPlan} isn't available on this PC");

        foreach (var app in AppsToOpen(next))
        {
            log($"  Opening {app.Name}");
            try { AppController.Launch(app); }
            catch (Exception ex) { log($"  ! Couldn't open {app.Name}: {ex.Message}"); }
        }

        log($"{next.Name} mode ready.");
    }

    static bool SameApp(AppEntry a, AppEntry b) =>
        string.Equals(AppController.ProcessNameOf(a), AppController.ProcessNameOf(b),
                      StringComparison.OrdinalIgnoreCase);
}
