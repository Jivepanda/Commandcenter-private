namespace CommandCentre.Models
{
    public class AppEntry
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public string Args { get; set; } = "";
        public string? ProcessName { get; set; }
    }

    public class Mode
    {
        public string Name { get; set; } = "";
        public string Accent { get; set; } = "#22d3ee";
        public string PowerPlan { get; set; } = "Balanced";
        public List<AppEntry> Apps { get; set; } = new();

        //drowpdown shows
        public override string ToString()
        {
            return Name;
        }
    }

    public class Config
    {
        public List<string> KeepAlways { get; set; } = new();
        public List<Mode> Modes { get; set; } = new();
    }
}
