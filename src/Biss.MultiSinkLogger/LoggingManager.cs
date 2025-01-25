using Biss.MultiSinkLogger.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Biss.MultiSinkLogger
{
    public static class LoggingManager
    {
        public static void InitializeLogger(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ConfigureLogging(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

    }
}
