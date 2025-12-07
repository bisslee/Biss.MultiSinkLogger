using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Interface para configuradores de sinks seguindo o Strategy Pattern.
    /// </summary>
    public interface ISinkConfigurator
    {
        /// <summary>
        /// Obtém o tipo de sink que este configurador suporta.
        /// </summary>
        SinkType SupportedSinkType { get; }

        /// <summary>
        /// Configura o sink no logger.
        /// </summary>
        /// <param name="loggerConfiguration">A configuração do logger Serilog.</param>
        /// <param name="settings">As configurações específicas do sink.</param>
        /// <param name="loggerConfig">As configurações gerais do logger.</param>
        void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig);

        /// <summary>
        /// Valida as configurações do sink.
        /// </summary>
        /// <param name="settings">As configurações a serem validadas.</param>
        /// <exception cref="ArgumentException">Lançada quando as configurações são inválidas.</exception>
        void ValidateSettings(ISinkSettings settings);
    }
}
