using System.Diagnostics;
using System.IO;
using CommandCentre.Models;

namespace CommandCentre.Services;

public static class AppController
{
    //the name Windows knows the running process by, not the name of the exe file
    public static string ProcessNameOf(AppEntry app) =>
        string.IsNullOrWhiteSpace(app.ProcessName)
            ? Path.GetFileNameWithoutExtension(Environment.ExpandEnvironmentVariables(app.Path))
            : app.ProcessName;

    public static bool IsRunning(AppEntry app) => Process.GetProcessesByName(ProcessNameOf(app)).Length > 0;

    public static void Launch(AppEntry app)
    {
        if (!IsRunning(app))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(app.Path),
                Arguments = Environment.ExpandEnvironmentVariables(app.Args),
                UseShellExecute = true
            });
        }
    }

    public static void AskToClose(AppEntry app)
    {
        foreach (var p in Process.GetProcessesByName(ProcessNameOf(app)))
        {
            if (p.MainWindowHandle != IntPtr.Zero) p.CloseMainWindow();
        }
    }
}
