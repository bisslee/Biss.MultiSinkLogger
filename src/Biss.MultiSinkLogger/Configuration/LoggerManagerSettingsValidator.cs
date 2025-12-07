using Biss.MultiSinkLogger.Entities;
using Microsoft.Extensions.Options;

namespace Biss.MultiSinkLogger.Configuration
{
    /// <summary>
    /// Validador para LoggerManagerSettings usando IValidateOptions.
    /// </summary>
    public class LoggerManagerSettingsValidator : IValidateOptions<LoggerManagerSettings>
    {
        /// <summary>
        /// Valida as configurações do LoggerManagerSettings.
        /// </summary>
        /// <param name="name">Nome da opção.</param>
        /// <param name="options">As opções a serem validadas.</param>
        /// <returns>Resultado da validação.</returns>
        public ValidateOptionsResult Validate(string? name, LoggerManagerSettings options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("LoggerManagerSettings não pode ser null.");
            }

            var errors = new List<string>();

            // Validar MinimumLevel
            if (string.IsNullOrWhiteSpace(options.MinimumLevel))
            {
                errors.Add("MinimumLevel é obrigatório.");
            }
            else if (!Enum.TryParse<Serilog.Events.LogEventLevel>(options.MinimumLevel, true, out _))
            {
                var validLevels = string.Join(", ", Enum.GetNames(typeof(Serilog.Events.LogEventLevel)));
                errors.Add($"MinimumLevel '{options.MinimumLevel}' não é um valor válido. Valores válidos: {validLevels}");
            }

            // Validar Sinks
            if (options.Sinks == null)
            {
                errors.Add("Sinks não pode ser null.");
            }
            else
            {
                var activeSinks = options.Sinks.Where(s => s.Active).ToList();

                if (!activeSinks.Any())
                {
                    // Não é um erro crítico, mas avisar que nenhum sink está ativo
                    // O sistema usará Console como padrão
                }

                for (int i = 0; i < options.Sinks.Count; i++)
                {
                    var sink = options.Sinks[i];
                    var sinkIndex = i;

                    if (string.IsNullOrWhiteSpace(sink.Type))
                    {
                        errors.Add($"Sink[{sinkIndex}]: Tipo não pode ser vazio.");
                    }
                    else
                    {
                        // Validar se o tipo é um enum válido
                        if (!Enum.TryParse<SinkType>(sink.Type, true, out _))
                        {
                            var validTypes = string.Join(", ", Enum.GetNames(typeof(SinkType)));
                            errors.Add($"Sink[{sinkIndex}]: Tipo '{sink.Type}' não é válido. Valores válidos: {validTypes}");
                        }
                    }

                    if (sink.Active && sink.Settings == null)
                    {
                        errors.Add($"Sink[{sinkIndex}] ({sink.Type}): Configurações não podem ser null para sinks ativos.");
                    }
                }
            }

            if (errors.Any())
            {
                return ValidateOptionsResult.Fail(errors);
            }

            return ValidateOptionsResult.Success;
        }
    }
}

