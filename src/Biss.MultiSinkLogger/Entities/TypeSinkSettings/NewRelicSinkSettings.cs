using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class NewRelicSinkSettings : ISinkSettings
    {
        public string EndpointUrl { get; set; } = "https://log-api.newrelic.com/log/v1"; // URL do New Relic        
        public string ApplicationName { get; set; } = null!;
        public string LicenseKey { get; set; } = null!;
        public string InsertKey { get; set; } = null!;
        public int BatchSizeLimit { get; set; } = 1000; // Opcional: tamanho do lote para inserções em massa
        public TimeSpan? Period { get; set; } = null; // Opcional: período de tempo para enviar logs em lote
        public string LogLevel { get; set; } = "Information"; // Opcional: define o nível mínimo de log a ser enviado
    }
}
