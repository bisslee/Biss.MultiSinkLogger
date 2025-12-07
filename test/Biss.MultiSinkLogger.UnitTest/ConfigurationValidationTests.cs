using Biss.MultiSinkLogger.Configuration;
using Biss.MultiSinkLogger.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para validação de configuração.
    /// </summary>
    public class ConfigurationValidationTests
    {
        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_Null_Settings()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();

            // Act
            var result = validator.Validate(null, null);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("não pode ser null", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_Invalid_MinimumLevel()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "InvalidLevel",
                Sinks = new List<Sink>()
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("MinimumLevel", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_Empty_MinimumLevel()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "",
                Sinks = new List<Sink>()
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("MinimumLevel", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_Null_Sinks()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "Information",
                Sinks = null!
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("Sinks", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_ActiveSink_Without_Settings()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "Information",
                Sinks = new List<Sink>
                {
                    new Sink
                    {
                        Type = "Console",
                        Active = true,
                        Settings = null!
                    }
                }
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("Configurações não podem ser null", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Reject_Invalid_SinkType()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "Information",
                Sinks = new List<Sink>
                {
                    new Sink
                    {
                        Type = "InvalidSinkType",
                        Active = true
                    }
                }
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Failed);
            Assert.Contains("Tipo", result.FailureMessage);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Accept_Valid_Configuration()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "Information",
                Sinks = new List<Sink>
                {
                    new Sink
                    {
                        Type = "Console",
                        Active = true,
                        Settings = new Entities.TypeSinkSettings.ConsoleSinkSettings()
                    }
                }
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void LoggerManagerSettingsValidator_Should_Accept_Empty_Sinks_List()
        {
            // Arrange
            var validator = new LoggerManagerSettingsValidator();
            var settings = new LoggerManagerSettings
            {
                MinimumLevel = "Information",
                Sinks = new List<Sink>()
            };

            // Act
            var result = validator.Validate(null, settings);

            // Assert
            // Lista vazia é aceita (sistema usará Console como padrão)
            Assert.True(result.Succeeded);
        }
    }
}

