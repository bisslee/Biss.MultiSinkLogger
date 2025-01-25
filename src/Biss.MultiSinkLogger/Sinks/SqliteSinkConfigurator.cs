using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class SqliteSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var sqliteSettings = settings as SqliteSinkSettings;

            loggerConfiguration.WriteTo.SQLite(
                sqliteDbPath: sqliteSettings?.ConnectionString,
                tableName: sqliteSettings?.TableName,
                storeTimestampInUtc: true
            );
        }
    }
}
