using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Biss.MultiSinkLogger.Sinks
{
    public class ConsoleSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var consoleSettings = settings as ConsoleSinkSettings;

            var theme = AnsiConsoleTheme.Code; // Padrão

            loggerConfiguration.WriteTo.Console(
                outputTemplate: loggerConfig.OutputTemplate,
                theme: theme
            );
        }
    }
}
