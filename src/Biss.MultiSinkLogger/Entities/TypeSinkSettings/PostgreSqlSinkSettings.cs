namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class PostgreSqlSinkSettings : ISinkSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int BatchSize { get; set; } = 50; // Opcional: tamanho do lote para inserções em massa
    }
}
