using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de Console.
    /// </summary>
    public class ConsoleSinkConfigurator : ISinkConfigurator
    {
        /// <summary>
        /// Obtém o tipo de sink suportado (Console).
        /// </summary>
        public SinkType SupportedSinkType => SinkType.Console;

        /// <summary>
        /// Configura o sink de console no logger.
        /// </summary>
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);

            var theme = AnsiConsoleTheme.Code; // Padrão

            loggerConfiguration.WriteTo.Console(
                outputTemplate: loggerConfig.OutputTemplate,
                theme: theme
            );
        }

        /// <summary>
        /// Valida as configurações do sink de console.
        /// </summary>
        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not ConsoleSinkSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(ConsoleSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }
        }
    }
}
