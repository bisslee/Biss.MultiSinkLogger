using Biss.MultiSinkLogger.Entities;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Fábrica para criar configuradores de sinks baseados no tipo do sink.
    /// </summary>
    public static class SinkConfiguratorFactory
    {
        /// <summary>
        /// Obtém o configurador correspondente ao tipo de sink especificado.
        /// </summary>
        /// <param name="sinkType">O tipo do sink.</param>
        /// <returns>O configurador do sink correspondente.</returns>
        /// <exception cref="NotSupportedException">Lançado se o tipo do sink não for suportado.</exception>
        public static ISinkConfigurator GetConfigurator(SinkType sinkType)
        {
            try
            {
                return sinkType switch
                {
                    SinkType.Console => new ConsoleSinkConfigurator(),
                    SinkType.File => new FileSinkConfigurator(),
                    SinkType.CosmosDB => new CosmosDBSinkConfigurator(),
                    SinkType.MongoDB => new MongoDBSinkConfigurator(),
                    SinkType.Sqlite => new SqliteSinkConfigurator(),
                    SinkType.SqlServer => new SqlServerSinkConfigurator(),
                    SinkType.PostgreSql => new PostgreSqlSinkConfigurator(),
                    SinkType.MySql => new MySqlSinkConfigurator(),
                    SinkType.RabbitMQ => new RabbitMQSinkConfigurator(),
                    SinkType.Sentry => new SentrySinkConfigurator(),
                    SinkType.Slack => new SlackSinkConfigurator(),
                    SinkType.NewRelic => new NewRelicSinkConfigurator(),
                    _ => throw new NotSupportedException($"Sink '{sinkType}' is not supported.")
                };
            }
            catch (NotSupportedException ex)
            {
                Log.Error(ex, $"The specified sink type '{sinkType}' is not supported.");
                throw;
            }
        }
    }
}
