using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de NewRelic.
    /// </summary>
    public class NewRelicSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.NewRelic;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var newRelicSettings = (NewRelicSinkSettings)settings;

            loggerConfiguration.WriteTo.NewRelicLogs(
                endpointUrl: newRelicSettings.EndpointUrl,
                insertKey: newRelicSettings.InsertKey,
                applicationName: newRelicSettings.ApplicationName,
                licenseKey: newRelicSettings.LicenseKey,
                batchSizeLimit: newRelicSettings.BatchSizeLimit,
                period: newRelicSettings.Period,
                restrictedToMinimumLevel: Enum.Parse<Serilog.Events.LogEventLevel>(newRelicSettings.LogLevel, true)
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not NewRelicSinkSettings newRelicSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(NewRelicSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(newRelicSettings.EndpointUrl))
                throw new ArgumentException("EndpointUrl é obrigatório.", nameof(settings));
        }
    }
}
