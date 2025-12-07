using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de SQL Server.
    /// </summary>
    public class SqlServerSinkConfigurator : ISinkConfigurator
    {
        /// <summary>
        /// Obtém o tipo de sink suportado (SqlServer).
        /// </summary>
        public SinkType SupportedSinkType => SinkType.SqlServer;

        /// <summary>
        /// Configura o sink de SQL Server no logger.
        /// </summary>
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);

            var sqlServerSettings = (SqlServerSinkSettings)settings;

            if (string.IsNullOrWhiteSpace(sqlServerSettings.ConnectionString))
                throw new ArgumentException("ConnectionString não pode ser vazio.", nameof(settings));

            if (string.IsNullOrWhiteSpace(sqlServerSettings.TableName))
                throw new ArgumentException("TableName não pode ser vazio.", nameof(settings));

            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = sqlServerSettings.TableName,
                AutoCreateSqlTable = true
            };

            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.LogEvent.DataLength = -1; // Máximo
            columnOptions.PrimaryKey = columnOptions.Id; // Use o Id como chave primária

            loggerConfiguration.WriteTo.MSSqlServer(
                connectionString: sqlServerSettings.ConnectionString,
                sinkOptions: sinkOptions,
                columnOptions: columnOptions
            );
        }

        /// <summary>
        /// Valida as configurações do sink de SQL Server.
        /// </summary>
        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not SqlServerSinkSettings sqlServerSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(SqlServerSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(sqlServerSettings.ConnectionString))
                throw new ArgumentException("ConnectionString é obrigatório.", nameof(settings));

            if (string.IsNullOrWhiteSpace(sqlServerSettings.TableName))
                throw new ArgumentException("TableName é obrigatório.", nameof(settings));
        }
    }
}
