using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class RabbitMQSinkSettings : ISinkSettings
    {
        public string HostName { get; set; } = null!;
        public string VirtualHost { get; set; } = "/";
        public string QueueName { get; set; } = "logs";
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Port { get; set; } = 5672; // Porta padrão do RabbitMQ
        public string ExchangeName { get; set; } = "logs";
        public int? QueueLimit { get; set; }
        public string? RestrictedToMinimumLevel { get; set; }   
    }
}
