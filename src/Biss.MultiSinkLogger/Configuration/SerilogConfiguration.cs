using Biss.MultiSinkLogger.Constants;
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Biss.MultiSinkLogger.Exceptions;
using Biss.MultiSinkLogger.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Biss.MultiSinkLogger.Configuration
{

    /// <summary>
    /// Configuração do ciclo de vida do Logger
    /// </summary>  
    public static class SerilogConfiguration
    {
        /// <summary>
        /// Configura o Logger
        /// </summary>
        /// <param name="configuration">Configuração</param>
        /// <returns>LoggerConfiguration</returns>
        public static LoggerConfiguration ConfigureLogging(
          this LoggerConfiguration loggerConfiguration,
          IConfiguration configuration)
        {
            var loggerConfig = GetLoggerManagerSettings(configuration);

            // Configura o nível mínimo de log
            loggerConfiguration.MinimumLevel.Is(
                Enum.Parse<Serilog.Events.LogEventLevel>
                (loggerConfig.MinimumLevel, true));

            if (!loggerConfig.Sinks.Any(s => s.Active))
            {
                loggerConfiguration
                  .WriteTo
                  .Console(outputTemplate: TemplateConstants.OUTPUT_TEMPLATE);

                return loggerConfiguration;
            }

            ConfigureSinkSettings(loggerConfiguration, loggerConfig);

            return loggerConfiguration;
        }

        private static LoggerManagerSettings GetLoggerManagerSettings(IConfiguration configuration)
        {
            var loggerConfig = configuration.GetSection(nameof(LoggerManagerSettings))
                .Get<LoggerManagerSettings>();

            if (loggerConfig == null)
                throw new LoggerConfigurationException("Configuração de Logger não encontrada.");

            SinkSettingsConfig(configuration, loggerConfig);

            return loggerConfig;
        }

        private static void SinkSettingsConfig(IConfiguration configuration, LoggerManagerSettings loggerConfig)
        {
            var sinksSectionPath = $"{nameof(LoggerManagerSettings)}:{nameof(LoggerManagerSettings.Sinks)}";
            var sinksSections = configuration.GetSection(sinksSectionPath).GetChildren();

            foreach (var sinkSection in sinksSections)
            {
                var sink = new Sink
                {
                    Type = sinkSection.GetValue<string>(nameof(Sink.Type)) ?? string.Empty,
                    Active = sinkSection.GetValue<bool>(nameof(Sink.Active)),
                };

                if (string.IsNullOrEmpty(sink.Type))
                    throw new LoggerConfigurationException("Tipo de Sink não encontrado.");

                if (!sink.Active)
                    continue;

                SinkType sinkType = sink.Type.ParseEnum<SinkType>();
                var sinkSettingsSection = sinkSection.GetSection(nameof(Sink.Settings));

                if (sinkSettingsSection == null)
                    throw new LoggerConfigurationException("Configuração de Sink não encontrada.");


                switch (sinkType)
                {
                    case SinkType.Console:
                        sink.Settings = sinkSettingsSection.Get<ConsoleSinkSettings>();
                        break;
                    case SinkType.File:
                        sink.Settings = sinkSettingsSection.Get<FileSinkSettings>();
                        break;
                    case SinkType.CosmosDB:
                        sink.Settings = sinkSettingsSection.Get<CosmosDBSinkSettings>();
                        break;
                    case SinkType.MongoDB:
                        sink.Settings = sinkSettingsSection.Get<MongoDBSinkSettings>();
                        break;
                    case SinkType.MySql:
                        sink.Settings = sinkSettingsSection.Get<MySqlSinkSettings>();
                        break;
                    case SinkType.NewRelic:
                        sink.Settings = sinkSettingsSection.Get<NewRelicSinkSettings>();
                        break;
                    case SinkType.PostgreSql:
                        sink.Settings = sinkSettingsSection.Get<PostgreSqlSinkSettings>();
                        break;
                    case SinkType.RabbitMQ:
                        sink.Settings = sinkSettingsSection.Get<RabbitMQSinkSettings>();
                        break;
                    case SinkType.Sentry:
                        sink.Settings = sinkSettingsSection.Get<SentrySinkSettings>();
                        break;
                    case SinkType.Slack:
                        sink.Settings = sinkSettingsSection.Get<SlackSinkSettings>();
                        break;
                    case SinkType.SqlServer:
                        sink.Settings = sinkSettingsSection.Get<SqlServerSinkSettings>();
                        break;
                    case SinkType.Sqlite:
                        sink.Settings = sinkSettingsSection.Get<SqliteSinkSettings>();
                        break;
                    default:
                        throw new NotSupportedException($"O Configurador de Sink '{sink.Type}' não é suportado.");
                }

                loggerConfig.Sinks.Add(sink);
            }
        }

        private static void ConfigureSinkSettings(LoggerConfiguration loggerConfiguration, LoggerManagerSettings loggerConfig)
        {
            var sinks = loggerConfig.Sinks.Where(s => s.Active).ToList();

            foreach (var sink in sinks)
            {
                SinkType sinkType = sink.Type.ParseEnum<SinkType>();
                var configurator = SinkConfiguratorFactory.GetConfigurator(sinkType);
                configurator.Configure(loggerConfiguration, sink.Settings, loggerConfig);
            }
        }
    }
}