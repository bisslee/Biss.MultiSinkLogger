using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de MySQL.
    /// </summary>
    public class MySqlSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.MySql;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var mySqlSettings = (MySqlSinkSettings)settings;

            loggerConfiguration.WriteTo.MySQL(
                connectionString: mySqlSettings.ConnectionString,
                tableName: mySqlSettings.TableName
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not MySqlSinkSettings mySqlSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(MySqlSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(mySqlSettings.ConnectionString))
                throw new ArgumentException("ConnectionString é obrigatório.", nameof(settings));
        }
    }
}
