using Biss.MultiSinkLogger.Entities;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Fábrica para obter configuradores de sinks usando Strategy Pattern e DI.
    /// </summary>
    public class SinkConfiguratorFactory
    {
        private readonly Dictionary<SinkType, ISinkConfigurator> _configurators;

        /// <summary>
        /// Inicializa a factory com os configuradores disponíveis via DI.
        /// </summary>
        /// <param name="configurators">Coleção de configuradores registrados.</param>
        public SinkConfiguratorFactory(IEnumerable<ISinkConfigurator> configurators)
        {
            if (configurators == null)
                throw new ArgumentNullException(nameof(configurators));

            _configurators = configurators.ToDictionary(c => c.SupportedSinkType, c => c);
        }

        /// <summary>
        /// Obtém o configurador correspondente ao tipo de sink especificado.
        /// </summary>
        /// <param name="sinkType">O tipo do sink.</param>
        /// <returns>O configurador do sink correspondente.</returns>
        /// <exception cref="NotSupportedException">Lançado se o tipo do sink não for suportado.</exception>
        public ISinkConfigurator GetConfigurator(SinkType sinkType)
        {
            if (!_configurators.TryGetValue(sinkType, out var configurator))
            {
                var supportedTypes = string.Join(", ", _configurators.Keys);
                throw new NotSupportedException(
                    $"Sink '{sinkType}' não é suportado. Sinks disponíveis: {supportedTypes}");
            }

            return configurator;
        }

        /// <summary>
        /// Método estático para compatibilidade com versões anteriores (deprecated).
        /// </summary>
        /// <param name="sinkType">O tipo do sink.</param>
        /// <returns>O configurador do sink correspondente.</returns>
        /// <exception cref="NotSupportedException">Lançado se o tipo do sink não for suportado.</exception>
        [Obsolete("Use dependency injection instead. Register ISinkConfigurator implementations and inject SinkConfiguratorFactory.")]
        public static ISinkConfigurator GetConfiguratorStatic(SinkType sinkType)
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
