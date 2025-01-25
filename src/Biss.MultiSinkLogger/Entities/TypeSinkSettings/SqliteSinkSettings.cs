namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class SqliteSinkSettings : ISinkSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }
}
