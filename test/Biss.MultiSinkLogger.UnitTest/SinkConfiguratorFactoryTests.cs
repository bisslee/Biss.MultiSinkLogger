using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Sinks;
using Moq;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para SinkConfiguratorFactory.
    /// </summary>
    public class SinkConfiguratorFactoryTests
    {
        [Fact]
        public void GetConfigurator_Should_Return_Correct_Configurator()
        {
            // Arrange
            var consoleConfigurator = new ConsoleSinkConfigurator();
            var fileConfigurator = new FileSinkConfigurator();
            var configurators = new ISinkConfigurator[]
            {
                consoleConfigurator,
                fileConfigurator
            };

            var factory = new SinkConfiguratorFactory(configurators);

            // Act
            var result = factory.GetConfigurator(SinkType.Console);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(SinkType.Console, result.SupportedSinkType);
        }

        [Fact]
        public void GetConfigurator_With_Unsupported_Type_Should_Throw()
        {
            // Arrange
            var configurators = new ISinkConfigurator[]
            {
                new ConsoleSinkConfigurator()
            };

            var factory = new SinkConfiguratorFactory(configurators);

            // Act & Assert
            var exception = Assert.Throws<NotSupportedException>(() =>
                factory.GetConfigurator(SinkType.MongoDB));

            Assert.Contains("não é suportado", exception.Message);
        }

        [Fact]
        public void GetConfigurator_With_Null_Configurators_Should_Throw()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SinkConfiguratorFactory(null!));
        }

        [Fact]
        public void GetConfiguratorStatic_Should_Return_Configurator()
        {
            // Arrange & Act
            var configurator = SinkConfiguratorFactory.GetConfiguratorStatic(SinkType.Console);

            // Assert
            Assert.NotNull(configurator);
            Assert.Equal(SinkType.Console, configurator.SupportedSinkType);
        }

        [Fact]
        public void GetConfiguratorStatic_With_Unsupported_Type_Should_Throw()
        {
            // Arrange & Act & Assert
            // Nota: Como não temos todos os tipos no enum, vamos testar com um tipo válido
            // mas que pode não ter implementação no método estático
            var exception = Assert.Throws<NotSupportedException>(() =>
                SinkConfiguratorFactory.GetConfiguratorStatic((SinkType)999));

            Assert.Contains("not supported", exception.Message);
        }

        [Fact]
        public void Factory_Should_Handle_Multiple_Configurators()
        {
            // Arrange
            var configurators = new ISinkConfigurator[]
            {
                new ConsoleSinkConfigurator(),
                new FileSinkConfigurator(),
                new SqlServerSinkConfigurator()
            };

            var factory = new SinkConfiguratorFactory(configurators);

            // Act
            var console = factory.GetConfigurator(SinkType.Console);
            var file = factory.GetConfigurator(SinkType.File);
            var sqlServer = factory.GetConfigurator(SinkType.SqlServer);

            // Assert
            Assert.NotNull(console);
            Assert.NotNull(file);
            Assert.NotNull(sqlServer);
            Assert.Equal(SinkType.Console, console.SupportedSinkType);
            Assert.Equal(SinkType.File, file.SupportedSinkType);
            Assert.Equal(SinkType.SqlServer, sqlServer.SupportedSinkType);
        }
    }
}

