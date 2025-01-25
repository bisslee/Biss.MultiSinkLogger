using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Biss.MultiSinkLogger.Sinks
{
    public class SqlServerSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var sqlServerSettings = settings as SqlServerSinkSettings;

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
    }
}
