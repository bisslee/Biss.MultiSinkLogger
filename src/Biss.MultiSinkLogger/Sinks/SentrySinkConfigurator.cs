using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class SentrySinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var sentrySettings = settings as SentrySinkSettings;

            if (sentrySettings != null)
            {
                loggerConfiguration.WriteTo.Sentry(
                    dsn: sentrySettings.Dsn,
                    environment: sentrySettings.Environment,
                    restrictedToMinimumLevel: Enum.Parse<Serilog.Events.LogEventLevel>(sentrySettings.LogLevel, true)
                );
            }
        }
    }
}
