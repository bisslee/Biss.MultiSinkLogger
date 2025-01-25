using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.Slack;

namespace Biss.MultiSinkLogger.Sinks
{
    public class SlackSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var slackSettings = settings as SlackSinkSettings;

            if (slackSettings != null)
            {

                loggerConfiguration.WriteTo.Slack(
                    webhookUrl: slackSettings.WebhookUrl,
                    customChannel: slackSettings.Channel,
                    customUsername: slackSettings.Username,
                    customIcon: slackSettings.IconEmoji
                );
            }

        }
    }
}
