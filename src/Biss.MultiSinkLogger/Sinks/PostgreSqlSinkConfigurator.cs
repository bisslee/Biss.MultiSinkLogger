using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de PostgreSQL.
    /// </summary>
    public class PostgreSqlSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.PostgreSql;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var postgreSqlSettings = (PostgreSqlSinkSettings)settings;

            loggerConfiguration.WriteTo.PostgreSQL(
                connectionString: postgreSqlSettings.ConnectionString,
                tableName: postgreSqlSettings.TableName,
                needAutoCreateTable: true,
                batchSizeLimit: postgreSqlSettings.BatchSize
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not PostgreSqlSinkSettings postgreSqlSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(PostgreSqlSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(postgreSqlSettings.ConnectionString))
                throw new ArgumentException("ConnectionString é obrigatório.", nameof(settings));
        }
    }
}
