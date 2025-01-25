using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class MySqlSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var mySqlSettings = settings as MySqlSinkSettings;

            loggerConfiguration.WriteTo.MySQL(
                connectionString: mySqlSettings.ConnectionString,
                tableName: mySqlSettings.TableName

            );
        }
    }
}
