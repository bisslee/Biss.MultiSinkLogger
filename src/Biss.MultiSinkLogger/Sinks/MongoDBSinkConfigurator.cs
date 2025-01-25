using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class MongoDBSinkConfigurator : ISinkConfigurator
    {
        public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings, LoggerManagerSettings loggerConfig)
        {
            var mongoSettings = settings as MongoDBSinkSettings;

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
    }
}
