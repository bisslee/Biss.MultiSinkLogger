using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class FileSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var fileSettings = settings as FileSinkSettings;

            var filePath = Path.Combine(fileSettings.Path, fileSettings.Filename);

            loggerConfiguration.WriteTo.File(
                path: filePath,
                rollingInterval: Enum.Parse<RollingInterval>(fileSettings.RollingInterval),
                retainedFileCountLimit: fileSettings.RetainedFileCountLimit,
                outputTemplate: loggerConfig.OutputTemplate,
                shared: fileSettings.Shared
            );
        }
    }
}
