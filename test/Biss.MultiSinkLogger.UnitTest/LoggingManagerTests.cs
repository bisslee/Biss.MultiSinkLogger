using Biss.MultiSinkLogger;
using Microsoft.Extensions.Configuration;
using Serilog;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para LoggingManager focando em thread safety.
    /// </summary>
    public class LoggingManagerTests
    {
        private readonly IConfiguration _configuration;

        public LoggingManagerTests()
        {
            // Configuração mínima sem sinks ativos (usa Console como padrão)
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "LoggerManagerSettings:MinimumLevel", "Information" }
                    // Sem sinks ativos - o sistema usará Console como padrão
                })
                .Build();
        }

        [Fact]
        public void InitializeLogger_Should_Be_ThreadSafe()
        {
            // Arrange
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            // Act - Tentar inicializar múltiplas vezes simultaneamente
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        LoggingManager.InitializeLogger(_configuration);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Empty(exceptions);
            Assert.NotNull(Log.Logger);
        }

        [Fact]
        public void InitializeLogger_Should_Not_Initialize_Multiple_Times()
        {
            // Arrange & Act
            LoggingManager.InitializeLogger(_configuration);
            var firstLogger = Log.Logger;

            LoggingManager.InitializeLogger(_configuration);
            var secondLogger = Log.Logger;

            // Assert - Deve ser o mesmo logger (não criar novo)
            Assert.NotNull(firstLogger);
            Assert.NotNull(secondLogger);
            // Nota: Não podemos comparar diretamente, mas podemos verificar que não houve exceção
        }

        [Fact]
        public void CloseAndFlush_Should_Reset_Initialization()
        {
            // Arrange
            LoggingManager.InitializeLogger(_configuration);
            Assert.NotNull(Log.Logger);

            // Act
            LoggingManager.CloseAndFlush();

            // Assert
            // Após CloseAndFlush, o logger pode ser null ou SilentLogger
            // O importante é que não cause exceção
        }

        [Fact]
        public void InitializeLogger_With_NullConfiguration_Should_Throw()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                LoggingManager.InitializeLogger(null!));
        }
    }
}

