using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de SQLite.
    /// </summary>
    public class SqliteSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.Sqlite;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var sqliteSettings = (SqliteSinkSettings)settings;

            loggerConfiguration.WriteTo.SQLite(
                sqliteDbPath: sqliteSettings.ConnectionString,
                tableName: sqliteSettings.TableName,
                storeTimestampInUtc: true
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not SqliteSinkSettings sqliteSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(SqliteSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(sqliteSettings.ConnectionString))
                throw new ArgumentException("ConnectionString é obrigatório.", nameof(settings));
        }
    }
}
