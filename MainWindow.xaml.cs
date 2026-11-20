using System.Windows;
using CommandCentre.Models;
using CommandCentre.Services;

namespace CommandCentre;

public partial class MainWindow : Window
{
    readonly Config config;
    readonly ModeManager modes;

    public MainWindow()
    {
        InitializeComponent();
        config = ConfigStore.Load();
        modes = new ModeManager(config, Log);
        ModePicker.ItemsSource = config.Modes;
        ModePicker.SelectedIndex = 0;
        Log($"Loaded {config.Modes.Count} modes.");
    }

    void Log(string message) =>
        LogBox.AppendText($"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");

    static string ListOf(IEnumerable<AppEntry> apps) =>
        apps.Any() ? string.Join(", ", apps.Select(a => a.Name)) : "nothing";

    async void SwitchButton_Click(object sender, RoutedEventArgs e)
    {
        if (ModePicker.SelectedItem is not Mode next) return;

        var answer = MessageBox.Show(
            $"Will close: {ListOf(modes.AppsToClose(next))}\nWill open: {ListOf(modes.AppsToOpen(next))}",
            $"Switch to {next.Name}?", MessageBoxButton.OKCancel);
        if (answer != MessageBoxResult.OK) return;

        SwitchButton.IsEnabled = false;
        await modes.SwitchTo(next);
        SwitchButton.IsEnabled = true;
    }
}