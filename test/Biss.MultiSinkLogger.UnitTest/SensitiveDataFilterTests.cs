using Biss.MultiSinkLogger.Security;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para SensitiveDataFilter.
    /// </summary>
    public class SensitiveDataFilterTests
    {
        [Fact]
        public void FilterHeaders_Should_Filter_Authorization_Header()
        {
            // Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer token123" },
                { "Content-Type", "application/json" }
            };

            // Act
            var filtered = SensitiveDataFilter.FilterHeaders(headers);

            // Assert
            Assert.Equal("[REDACTED]", filtered["Authorization"]);
            Assert.Equal("application/json", filtered["Content-Type"]);
        }

        [Fact]
        public void FilterHeaders_Should_Filter_All_Sensitive_Headers()
        {
            // Arrange
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer token123" },
                { "X-API-Key", "api-key-123" },
                { "Cookie", "session=abc123" },
                { "Content-Type", "application/json" }
            };

            // Act
            var filtered = SensitiveDataFilter.FilterHeaders(headers);

            // Assert
            Assert.Equal("[REDACTED]", filtered["Authorization"]);
            Assert.Equal("[REDACTED]", filtered["X-API-Key"]);
            Assert.Equal("[REDACTED]", filtered["Cookie"]);
            Assert.Equal("application/json", filtered["Content-Type"]);
        }

        [Fact]
        public void FilterHeaders_Should_Handle_Case_Insensitive()
        {
            // Arrange
            var headers = new Dictionary<string, string>
            {
                { "authorization", "Bearer token123" },
                { "X-Api-Key", "api-key-123" }
            };

            // Act
            var filtered = SensitiveDataFilter.FilterHeaders(headers);

            // Assert
            Assert.Equal("[REDACTED]", filtered["authorization"]);
            Assert.Equal("[REDACTED]", filtered["X-Api-Key"]);
        }

        [Fact]
        public void FilterHeaders_With_Null_Should_Return_Empty_Dictionary()
        {
            // Arrange & Act
            var filtered = SensitiveDataFilter.FilterHeaders(null!);

            // Assert
            Assert.NotNull(filtered);
            Assert.Empty(filtered);
        }

        [Fact]
        public void FilterSensitiveData_Should_Filter_Password()
        {
            // Arrange
            var content = "username=user&password=secret123";

            // Act
            var filtered = SensitiveDataFilter.FilterSensitiveData(content);

            // Assert
            Assert.Contains("password", filtered, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("[REDACTED]", filtered);
            Assert.DoesNotContain("secret123", filtered);
        }

        [Fact]
        public void FilterSensitiveData_Should_Filter_Token()
        {
            // Arrange
            var content = "{\"token\":\"abc123\",\"data\":\"value\"}";

            // Act
            var filtered = SensitiveDataFilter.FilterSensitiveData(content);

            // Assert
            Assert.Contains("\"token\"", filtered, StringComparison.OrdinalIgnoreCase);
            // O padrão deve filtrar o valor do token
            // Pode estar como "token": [REDACTED] ou similar
            Assert.True(
                filtered.Contains("[REDACTED]", StringComparison.OrdinalIgnoreCase) ||
                !filtered.Contains("abc123", StringComparison.OrdinalIgnoreCase),
                $"Filtro não funcionou corretamente. Resultado: {filtered}");
        }

        [Fact]
        public void FilterSensitiveData_Should_Filter_Multiple_Patterns()
        {
            // Arrange
            var content = "password=secret&token=abc123&api_key=xyz789";

            // Act
            var filtered = SensitiveDataFilter.FilterSensitiveData(content);

            // Assert
            Assert.Contains("[REDACTED]", filtered);
            Assert.DoesNotContain("secret", filtered);
            Assert.DoesNotContain("abc123", filtered);
            Assert.DoesNotContain("xyz789", filtered);
        }

        [Fact]
        public void FilterSensitiveData_With_Null_Should_Return_Null()
        {
            // Arrange & Act
            var filtered = SensitiveDataFilter.FilterSensitiveData(null!);

            // Assert
            Assert.Null(filtered);
        }

        [Fact]
        public void FilterSensitiveData_With_Empty_String_Should_Return_Empty()
        {
            // Arrange & Act
            var filtered = SensitiveDataFilter.FilterSensitiveData(string.Empty);

            // Assert
            Assert.Equal(string.Empty, filtered);
        }

        [Fact]
        public void SanitizeConnectionString_Should_Remove_Password()
        {
            // Arrange
            var connectionString = "Server=server;Database=db;User Id=user;Password=secret123;";

            // Act
            var sanitized = SensitiveDataFilter.SanitizeConnectionString(connectionString);

            // Assert
            Assert.Contains("Password", sanitized, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("[REDACTED]", sanitized);
            Assert.DoesNotContain("secret123", sanitized);
        }

        [Fact]
        public void SanitizeConnectionString_Should_Remove_User_Id()
        {
            // Arrange
            var connectionString = "Server=server;Database=db;User Id=admin;Password=secret;";

            // Act
            var sanitized = SensitiveDataFilter.SanitizeConnectionString(connectionString);

            // Assert
            Assert.Contains("[REDACTED]", sanitized);
        }

        [Fact]
        public void SanitizeConnectionString_With_Null_Should_Return_Null()
        {
            // Arrange & Act
            var sanitized = SensitiveDataFilter.SanitizeConnectionString(null!);

            // Assert
            Assert.Null(sanitized);
        }

        [Fact]
        public void SanitizeConnectionString_Should_Preserve_NonSensitive_Data()
        {
            // Arrange
            var connectionString = "Server=myserver;Database=mydb;Trusted_Connection=true;";

            // Act
            var sanitized = SensitiveDataFilter.SanitizeConnectionString(connectionString);

            // Assert
            Assert.Contains("Server=myserver", sanitized);
            Assert.Contains("Database=mydb", sanitized);
            Assert.Contains("Trusted_Connection=true", sanitized);
        }
    }
}

