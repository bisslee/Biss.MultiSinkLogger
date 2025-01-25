namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class MongoDBSinkSettings : ISinkSettings
    {
        public string DatabaseUrl { get; set; } = null!;
        public string CollectionName { get; set; } = null!;
        public int CappedMaxSizeMb { get; set; }
        public int CappedMaxDocuments { get; set; }
        public string RollingInterval { get; set; } = "Infinite";
    }
}
