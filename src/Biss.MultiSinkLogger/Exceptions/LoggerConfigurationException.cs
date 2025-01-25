using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biss.MultiSinkLogger.Exceptions
{
    public class LoggerConfigurationException: Exception
    {
        const string DefaultMessage = "Logger configuration is invalid";

        public LoggerConfigurationException(string message) : base(message)
        {
        }

        public LoggerConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoggerConfigurationException(): base(DefaultMessage)
        {

        }
    }
}
