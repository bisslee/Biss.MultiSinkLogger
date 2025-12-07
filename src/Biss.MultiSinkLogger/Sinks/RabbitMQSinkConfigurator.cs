using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.RabbitMQ;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de RabbitMQ.
    /// </summary>
    public class RabbitMQSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.RabbitMQ;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var rabbitMQSettings = (RabbitMQSinkSettings)settings;

            loggerConfiguration.WriteTo.RabbitMQ(
                (clientConfiguration, sinkConfiguration) =>
                {
                    // Configurações do cliente RabbitMQ
                    clientConfiguration.Hostnames = new List<string>()
                    {
                        rabbitMQSettings.HostName,
                    };

                    clientConfiguration.Username = rabbitMQSettings.UserName;
                    clientConfiguration.Password = rabbitMQSettings.Password;
                    clientConfiguration.Port = rabbitMQSettings.Port;

                    // Configurações do sink                    
                    sinkConfiguration.QueueLimit = rabbitMQSettings.QueueLimit;
                    sinkConfiguration.RestrictedToMinimumLevel = Enum.Parse<Serilog.Events.LogEventLevel>(rabbitMQSettings.RestrictedToMinimumLevel, true);
                });
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not RabbitMQSinkSettings rabbitMQSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(RabbitMQSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(rabbitMQSettings.HostName))
                throw new ArgumentException("HostName é obrigatório.", nameof(settings));
        }
    }
}
