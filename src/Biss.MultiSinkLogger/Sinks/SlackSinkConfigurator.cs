using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.Slack;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de Slack.
    /// </summary>
    public class SlackSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.Slack;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var slackSettings = (SlackSinkSettings)settings;

            loggerConfiguration.WriteTo.Slack(
                webhookUrl: slackSettings.WebhookUrl,
                customChannel: slackSettings.Channel,
                customUsername: slackSettings.Username,
                customIcon: slackSettings.IconEmoji
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not SlackSinkSettings slackSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(SlackSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(slackSettings.WebhookUrl))
                throw new ArgumentException("WebhookUrl é obrigatório.", nameof(settings));
        }
    }
}
