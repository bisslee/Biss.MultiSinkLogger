using Microsoft.Extensions.Options;

namespace Biss.MultiSinkLogger.Configuration
{
    /// <summary>
    /// Validador para LoggingMiddlewareSettings.
    /// </summary>
    public class LoggingMiddlewareSettingsValidator : IValidateOptions<LoggingMiddlewareSettings>
    {
        /// <summary>
        /// Valida as configurações do middleware de logging.
        /// </summary>
        /// <param name="name">Nome da opção (não usado).</param>
        /// <param name="options">As opções a serem validadas.</param>
        /// <returns>Resultado da validação.</returns>
        public ValidateOptionsResult Validate(string? name, LoggingMiddlewareSettings options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("LoggingMiddlewareSettings não pode ser nulo.");
            }

            if (options.MaxBodyLength <= 0)
            {
                return ValidateOptionsResult.Fail(
                    $"MaxBodyLength deve ser maior que zero. Valor atual: {options.MaxBodyLength}");
            }

            // Limitar tamanho máximo razoável para evitar problemas de memória
            const int maxReasonableSize = 10_000_000; // 10MB
            if (options.MaxBodyLength > maxReasonableSize)
            {
                return ValidateOptionsResult.Fail(
                    $"MaxBodyLength não pode ser maior que {maxReasonableSize:N0}. Valor atual: {options.MaxBodyLength}");
            }

            return ValidateOptionsResult.Success;
        }
    }
}

