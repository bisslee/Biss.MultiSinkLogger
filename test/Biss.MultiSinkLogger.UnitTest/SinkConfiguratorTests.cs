using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Biss.MultiSinkLogger.Sinks;
using Serilog;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para os configuradores de sink.
    /// </summary>
    public class SinkConfiguratorTests
    {
        [Fact]
        public void ConsoleSinkConfigurator_Should_Have_Correct_SupportedType()
        {
            // Arrange
            var configurator = new ConsoleSinkConfigurator();

            // Act & Assert
            Assert.Equal(SinkType.Console, configurator.SupportedSinkType);
        }

        [Fact]
        public void ConsoleSinkConfigurator_Should_Validate_Settings()
        {
            // Arrange
            var configurator = new ConsoleSinkConfigurator();
            var validSettings = new ConsoleSinkSettings();
            var invalidSettings = new FileSinkSettings();

            // Act & Assert
            configurator.ValidateSettings(validSettings);

            var exception = Assert.Throws<ArgumentException>(() =>
                configurator.ValidateSettings(invalidSettings));

            Assert.Contains("ConsoleSinkSettings", exception.Message);
        }

        [Fact]
        public void FileSinkConfigurator_Should_Validate_Required_Properties()
        {
            // Arrange
            var configurator = new FileSinkConfigurator();
            var settingsWithoutPath = new FileSinkSettings
            {
                Path = "",
                Filename = "test.log"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                configurator.ValidateSettings(settingsWithoutPath));

            Assert.Contains("Path", exception.Message);
        }

        [Fact]
        public void FileSinkConfigurator_Should_Validate_Filename()
        {
            // Arrange
            var configurator = new FileSinkConfigurator();
            var settingsWithoutFilename = new FileSinkSettings
            {
                Path = "C:\\logs",
                Filename = ""
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                configurator.ValidateSettings(settingsWithoutFilename));

            Assert.Contains("Filename", exception.Message);
        }

        [Fact]
        public void SqlServerSinkConfigurator_Should_Validate_ConnectionString()
        {
            // Arrange
            var configurator = new SqlServerSinkConfigurator();
            var settingsWithoutConnectionString = new SqlServerSinkSettings
            {
                ConnectionString = "",
                TableName = "Logs"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                configurator.ValidateSettings(settingsWithoutConnectionString));

            Assert.Contains("ConnectionString", exception.Message);
        }

        [Fact]
        public void SqlServerSinkConfigurator_Should_Validate_TableName()
        {
            // Arrange
            var configurator = new SqlServerSinkConfigurator();
            var settingsWithoutTableName = new SqlServerSinkSettings
            {
                ConnectionString = "Server=localhost;Database=Test;",
                TableName = ""
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                configurator.ValidateSettings(settingsWithoutTableName));

            Assert.Contains("TableName", exception.Message);
        }

        [Fact]
        public void All_Configurators_Should_Implement_SupportedSinkType()
        {
            // Arrange & Act & Assert
            Assert.Equal(SinkType.Console, new ConsoleSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.File, new FileSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.SqlServer, new SqlServerSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.Sqlite, new SqliteSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.PostgreSql, new PostgreSqlSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.MySql, new MySqlSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.MongoDB, new MongoDBSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.CosmosDB, new CosmosDBSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.RabbitMQ, new RabbitMQSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.Sentry, new SentrySinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.Slack, new SlackSinkConfigurator().SupportedSinkType);
            Assert.Equal(SinkType.NewRelic, new NewRelicSinkConfigurator().SupportedSinkType);
        }
    }
}

