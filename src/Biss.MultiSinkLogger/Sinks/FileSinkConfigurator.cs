using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de Arquivo.
    /// </summary>
    public class FileSinkConfigurator : ISinkConfigurator
    {
        /// <summary>
        /// Obtém o tipo de sink suportado (File).
        /// </summary>
        public SinkType SupportedSinkType => SinkType.File;

        /// <summary>
        /// Configura o sink de arquivo no logger.
        /// </summary>
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);

            var fileSettings = (FileSinkSettings)settings;

            if (string.IsNullOrWhiteSpace(fileSettings.Path))
                throw new ArgumentException("Path não pode ser vazio.", nameof(settings));

            if (string.IsNullOrWhiteSpace(fileSettings.Filename))
                throw new ArgumentException("Filename não pode ser vazio.", nameof(settings));

            var filePath = Path.Combine(fileSettings.Path, fileSettings.Filename);

            loggerConfiguration.WriteTo.File(
                path: filePath,
                rollingInterval: Enum.Parse<RollingInterval>(fileSettings.RollingInterval),
                retainedFileCountLimit: fileSettings.RetainedFileCountLimit,
                outputTemplate: loggerConfig.OutputTemplate,
                shared: fileSettings.Shared
            );
        }

        /// <summary>
        /// Valida as configurações do sink de arquivo.
        /// </summary>
        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not FileSinkSettings fileSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(FileSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(fileSettings.Path))
                throw new ArgumentException("Path é obrigatório.", nameof(settings));

            if (string.IsNullOrWhiteSpace(fileSettings.Filename))
                throw new ArgumentException("Filename é obrigatório.", nameof(settings));
        }
    }
}
