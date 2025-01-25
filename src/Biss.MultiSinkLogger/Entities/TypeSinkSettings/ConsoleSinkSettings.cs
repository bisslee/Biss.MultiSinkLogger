namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class ConsoleSinkSettings : ISinkSettings
    {
        public string Theme { get; set; } = "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console";
    }
}
