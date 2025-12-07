using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de CosmosDB.
    /// </summary>
    public class CosmosDBSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.CosmosDB;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var cosmosSettings = (CosmosDBSinkSettings)settings;

            var uri = new Uri(cosmosSettings.EndpointUrl);

            loggerConfiguration.WriteTo.AzCosmosDB(
                endpointUri: uri,
                authorizationKey: cosmosSettings.AuthorizationKey,
                databaseName: cosmosSettings.DatabaseName,
                collectionName: cosmosSettings.ContainerName,
                timeToLive: TimeSpan.FromDays(cosmosSettings.TimeToLive)
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not CosmosDBSinkSettings cosmosSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(CosmosDBSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(cosmosSettings.EndpointUrl))
                throw new ArgumentException("EndpointUrl é obrigatório.", nameof(settings));
        }
    }
}
