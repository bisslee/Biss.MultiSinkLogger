using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class SentrySinkSettings : ISinkSettings
    {
        public string Dsn { get; set; } = null!; // Data Source Name (URL do Sentry)
        public string Environment { get; set; } = "Production";
        public string LogLevel { get; set; } = "Error"; // Opcional: nível de severidade mínimo
    }
}
