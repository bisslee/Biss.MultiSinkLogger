using Biss.MultiSinkLogger.Entities;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para EnumParse.
    /// </summary>
    public class EnumParseTests
    {
        [Fact]
        public void ParseEnum_Should_Parse_Valid_Enum_Value()
        {
            // Arrange
            var value = "Console";

            // Act
            var result = value.ParseEnum<SinkType>();

            // Assert
            Assert.Equal(SinkType.Console, result);
        }

        [Fact]
        public void ParseEnum_Should_Be_Case_Insensitive()
        {
            // Arrange
            var value = "console";

            // Act
            var result = value.ParseEnum<SinkType>();

            // Assert
            Assert.Equal(SinkType.Console, result);
        }

        [Fact]
        public void ParseEnum_With_Invalid_Value_Should_Throw()
        {
            // Arrange
            var value = "InvalidSinkType";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                value.ParseEnum<SinkType>());

            Assert.Contains("não é um", exception.Message);
            Assert.Contains("SinkType", exception.Message);
        }

        [Fact]
        public void ParseEnum_With_Null_Should_Throw()
        {
            // Arrange
            string? value = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                value!.ParseEnum<SinkType>());
        }

        [Fact]
        public void ParseEnum_With_Empty_String_Should_Throw()
        {
            // Arrange
            var value = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                value.ParseEnum<SinkType>());

            Assert.Contains("não pode ser nulo ou vazio", exception.Message);
        }

        [Fact]
        public void ParseEnum_With_Whitespace_Should_Throw()
        {
            // Arrange
            var value = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                value.ParseEnum<SinkType>());

            Assert.Contains("não pode ser nulo ou vazio", exception.Message);
        }

        [Fact]
        public void TryParseEnum_Should_Return_True_For_Valid_Value()
        {
            // Arrange
            var value = "File";

            // Act
            var result = value.TryParseEnum<SinkType>(out var parsed);

            // Assert
            Assert.True(result);
            Assert.Equal(SinkType.File, parsed);
        }

        [Fact]
        public void TryParseEnum_Should_Return_False_For_Invalid_Value()
        {
            // Arrange
            var value = "InvalidType";

            // Act
            var result = value.TryParseEnum<SinkType>(out var parsed);

            // Assert
            Assert.False(result);
            Assert.Equal(default(SinkType), parsed);
        }

        [Fact]
        public void TryParseEnum_With_Null_Should_Return_False()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value!.TryParseEnum<SinkType>(out var parsed);

            // Assert
            Assert.False(result);
            Assert.Equal(default(SinkType), parsed);
        }

        [Fact]
        public void TryParseEnum_With_Empty_String_Should_Return_False()
        {
            // Arrange
            var value = "";

            // Act
            var result = value.TryParseEnum<SinkType>(out var parsed);

            // Assert
            Assert.False(result);
            Assert.Equal(default(SinkType), parsed);
        }
    }
}

