using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;

namespace Biss.MultiSinkLogger.HealthChecks
{
    /// <summary>
    /// Health check para verificar se o sistema de logging está funcionando corretamente.
    /// </summary>
    public class SinkHealthCheck : IHealthCheck
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Inicializa o health check.
        /// </summary>
        /// <param name="logger">Logger para verificação.</param>
        public SinkHealthCheck(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Verifica a saúde do sistema de logging.
        /// </summary>
        /// <param name="context">Contexto do health check.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado do health check.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar se o logger está inicializado
                if (Log.Logger == null)
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy("Logger não foi inicializado."));
                }

                // Testar escrita de log (usando um nível baixo para não poluir logs)
                _logger.Debug("Health check test log - {Timestamp}", DateTime.UtcNow);

                // Verificar se o logger está configurado corretamente
                var isConfigured = Log.Logger != null && 
                                   Log.Logger.GetType().Name != "SilentLogger";

                if (!isConfigured)
                {
                    return Task.FromResult(
                        HealthCheckResult.Degraded("Logger está inicializado mas pode não estar configurado corretamente."));
                }

                return Task.FromResult(
                    HealthCheckResult.Healthy("Logger está funcionando corretamente."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Erro ao verificar saúde do logger.", ex));
            }
        }
    }
}

