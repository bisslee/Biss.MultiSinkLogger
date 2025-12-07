using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    /// <summary>
    /// Configurador para o sink de MongoDB.
    /// </summary>
    public class MongoDBSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.MongoDB;

        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            var mongoSettings = (MongoDBSinkSettings)settings;

            var rollingInterval = Enum
                .Parse<Serilog.Sinks.MongoDB.RollingInterval>(mongoSettings.RollingInterval);

            loggerConfiguration.WriteTo.MongoDBBson(
                databaseUrl: mongoSettings.DatabaseUrl,
                collectionName: mongoSettings.CollectionName,
                cappedMaxSizeMb: mongoSettings.CappedMaxSizeMb,
                cappedMaxDocuments: mongoSettings.CappedMaxDocuments,
                rollingInterval: rollingInterval
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not MongoDBSinkSettings mongoSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(MongoDBSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(mongoSettings.DatabaseUrl))
                throw new ArgumentException("DatabaseUrl é obrigatório.", nameof(settings));
        }
    }
}
