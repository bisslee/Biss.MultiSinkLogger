using Biss.MultiSinkLogger.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Biss.MultiSinkLogger
{
    /// <summary>
    /// Gerenciador estático para inicialização thread-safe do logger.
    /// </summary>
    public static class LoggingManager
    {
        private static readonly object _lock = new object();
        private static bool _initialized = false;
        private static ILogger? _logger;

        /// <summary>
        /// Inicializa o logger de forma thread-safe.
        /// </summary>
        /// <param name="configuration">A configuração da aplicação.</param>
        /// <exception cref="ArgumentNullException">Lançada quando configuration é null.</exception>
        public static void InitializeLogger(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Double-check locking pattern para thread safety
            if (_initialized && _logger != null)
                return;

            lock (_lock)
            {
                if (_initialized && _logger != null)
                    return;

                _logger = new LoggerConfiguration()
                    .ConfigureLogging(configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                Log.Logger = _logger;
                _initialized = true;
            }
        }

        /// <summary>
        /// Fecha e libera recursos do logger de forma thread-safe.
        /// </summary>
        public static void CloseAndFlush()
        {
            lock (_lock)
            {
                if (_logger != null)
                {
                    Log.CloseAndFlush();
                    _logger = null;
                    _initialized = false;
                }
            }
        }
    }
}
