using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class NewRelicSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var newRelicSettings = settings as NewRelicSinkSettings;

            if (newRelicSettings != null)
            {
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
        }
    }
}
