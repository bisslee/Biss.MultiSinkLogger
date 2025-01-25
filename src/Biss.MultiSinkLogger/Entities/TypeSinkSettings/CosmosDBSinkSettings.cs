namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class CosmosDBSinkSettings : ISinkSettings
    {
        public string EndpointUrl { get; set; }
        public string AuthorizationKey { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public string PartitionKeyPath { get; set; }
        public int TimeToLive { get; set; } = -1; // -1 significa infinito
    }
}
