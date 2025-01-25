using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class CosmosDBSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var cosmosSettings = settings as CosmosDBSinkSettings;

            var uri = new Uri(cosmosSettings.EndpointUrl);

            loggerConfiguration.WriteTo.AzCosmosDB(
                endpointUri: uri,
                authorizationKey: cosmosSettings.AuthorizationKey,
                databaseName: cosmosSettings.DatabaseName,
                collectionName: cosmosSettings.ContainerName,
                timeToLive: TimeSpan.FromDays(cosmosSettings.TimeToLive)
            );
        }
    }
}
