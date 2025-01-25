namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class SqlServerSinkSettings : ISinkSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }
}

