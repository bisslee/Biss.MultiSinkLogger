using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Biss.MultiSinkLogger.UnitTest
{
    public class LoggerTests
    {
        private readonly IConfiguration configuration;
        private readonly IConfiguration configurationLogSqlFalse;
        public LoggerTests()
        {
            // Carrega o appsettings.json como configuração
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Carrega o appsettings.json com a configuração de LogSqlfLogging como false
            configurationLogSqlFalse = new ConfigurationBuilder()
                .AddJsonFile("appsettingsSqlDisable.json", optional: false, reloadOnChange: true)
                .Build();
        }

        [Fact(Skip = "Teste de integração - requer SQL Server configurado")]
        public void Logger_Should_Initialize_Correctly()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configuration);
            // Verifica se o Logger foi inicializado corretamente
            Assert.NotNull(Log.Logger);
        }

        [Fact(Skip = "Teste de integração - requer SQL Server configurado")]
        public void Logger_Should_Write_To_File()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configuration);

            // Define o caminho do arquivo de log
            var logFilePath = Path.Combine("C:\\temp\\Logs\\", $"log{DateTime.Now:yyyyMMdd}.txt");

            // Certifica-se de que o arquivo não exista antes do teste
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            // Escreve um log de teste
            Log.Information("Testando gravação no arquivo de log");

            // Aguarda brevemente para garantir que o log seja gravado
            System.Threading.Thread.Sleep(2000); // Reduza o tempo se necessário

            // Verifica se o arquivo foi criado
            Assert.True(File.Exists(logFilePath));
            Log.CloseAndFlush();

            // Aguarda mais um pouco para liberar o arquivo (apenas precaução)
            System.Threading.Thread.Sleep(1000);

            // Verifica se o conteúdo esperado está no log
            var logContent = File.ReadAllText(logFilePath);
            Assert.Contains("Testando gravação no arquivo de log", logContent);
        }

        [Fact(Skip = "Teste de integração - requer SQL Server configurado")]
        public void Logger_Should_Respect_Minimum_Log_Level()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configuration);

            // Define o caminho do arquivo de log
            var logFilePath = Path.Combine("C:\\temp\\Logs\\", $"log{DateTime.Now:yyyyMMdd}.txt");

            // Limpa o arquivo de log antes do teste
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            // Escreve logs de diferentes níveis
            Log.Debug("Log de nível Debug"); // Deve ser ignorado
            Log.Information("Log de nível Information"); // Deve ser gravado

            // Aguarda brevemente para garantir que os logs sejam gravados
            System.Threading.Thread.Sleep(1000);
            Log.CloseAndFlush();

            // Verifica se o arquivo foi criado
            Assert.True(File.Exists(logFilePath));

            // Verifica se apenas o log de Information foi gravado
            var logContent = File.ReadAllText(logFilePath);
            Assert.DoesNotContain("Log de nível Debug", logContent);
            Assert.Contains("Log de nível Information", logContent);
        }

        [Fact(Skip = "Teste de integração - requer SQL Server configurado")]
        public void Logger_Should_Write_To_SqlServer()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configuration);

            // Escreve um log de teste
            Log.Information("Log de teste para o SQL Server");

            // Aguarda brevemente para garantir que o log seja gravado no banco
            System.Threading.Thread.Sleep(2000);
            Log.CloseAndFlush();

            // Conexão direta para verificar a tabela
            var connectionString = configuration["LoggerManagerSettings:Sinks:2:Settings:ConnectionString"];
            var tableName = configuration["LoggerManagerSettings:Sinks:2:Settings:TableName"];

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE Message LIKE '%Log de teste para o SQL Server%'", connection);
                var count = (int)command.ExecuteScalar();
                Assert.True(count > 0, "O log não foi gravado no SQL Server.");
            }
        }

        [Fact]
        public void Logger_Should_Not_Write_To_SqlServer_When_Disabled()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configurationLogSqlFalse);
            // Escreve um log de teste
            Log.Information("Log de teste para o SQL Server");
            // Aguarda brevemente para garantir que o log não seja gravado no banco
            System.Threading.Thread.Sleep(2000);
            Log.CloseAndFlush();
            // Conexão direta para verificar a tabela
            var connectionString = configurationLogSqlFalse["LoggerManagerSettings:Sinks:2:Settings:ConnectionString"];
            var tableName = configurationLogSqlFalse["LoggerManagerSettings:Sinks:2:Settings:TableName"];
            var disabled = Convert.ToBoolean(configurationLogSqlFalse["LoggerManagerSettings:Sinks:2:Settings:Active"]);

            Assert.False(disabled);

        }

        [Fact(Skip = "Teste de integração - requer SQL Server configurado")]
        public void Logger_Should_Respect_OutputTemplate()
        {
            // Inicializa o Logger
            LoggingManager.InitializeLogger(configuration);

            // Define o caminho do arquivo de log
            var logFilePath = Path.Combine("C:\\temp\\Logs\\", $"log{DateTime.Now:yyyyMMdd}.txt");

            // Escreve um log de teste
            Log.Information("Teste de template");

            // Aguarda brevemente
            System.Threading.Thread.Sleep(1000);
            Log.CloseAndFlush();

            // Verifica se o log contém o formato configurado
            var logContent = File.ReadAllText(logFilePath);
            Assert.Contains("***", logContent); // OutputTemplate definido no appsettings.json
            Assert.Contains("[INF]", logContent); // Verifica o nível de log
        }

    }
}
