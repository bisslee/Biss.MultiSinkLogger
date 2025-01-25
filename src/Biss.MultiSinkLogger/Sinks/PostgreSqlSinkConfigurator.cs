using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Biss.MultiSinkLogger.Sinks
{
    public class PostgreSqlSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var postgreSqlSettings = settings as PostgreSqlSinkSettings;
            if (postgreSqlSettings != null)
            {
                loggerConfiguration.WriteTo.PostgreSQL(
                    connectionString: postgreSqlSettings.ConnectionString,
                    tableName: postgreSqlSettings.TableName,
                    needAutoCreateTable: true,
                    batchSizeLimit: postgreSqlSettings.BatchSize
                );
            }
        }
    }
}
