using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.RabbitMQ;

namespace Biss.MultiSinkLogger.Sinks
{
    public class RabbitMQSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var rabbitMQSettings = settings as RabbitMQSinkSettings;

            if (rabbitMQSettings != null)
            {
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
        }
    }
}
