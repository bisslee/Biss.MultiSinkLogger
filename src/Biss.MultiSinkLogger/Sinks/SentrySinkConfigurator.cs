using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de Sentry.
    /// </summary>
    public class SentrySinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.Sentry;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var sentrySettings = (SentrySinkSettings)settings;

            loggerConfiguration.WriteTo.Sentry(
                dsn: sentrySettings.Dsn,
                environment: sentrySettings.Environment,
                restrictedToMinimumLevel: Enum.Parse<Serilog.Events.LogEventLevel>(sentrySettings.LogLevel, true)
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not SentrySinkSettings sentrySettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(SentrySinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(sentrySettings.Dsn))
                throw new ArgumentException("Dsn é obrigatório.", nameof(settings));
        }
    }
}
