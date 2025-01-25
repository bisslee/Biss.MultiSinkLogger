namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class MySqlSinkSettings : ISinkSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string TableName { get; set; } = null!;
    }
}
