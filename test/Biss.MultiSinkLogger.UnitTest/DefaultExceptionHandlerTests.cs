using Biss.MultiSinkLogger.ExceptionHandlers;
using Microsoft.AspNetCore.Http;
using Moq;
using Serilog;
using Xunit;

namespace Biss.MultiSinkLogger.UnitTest
{
    /// <summary>
    /// Testes para DefaultExceptionHandler.
    /// </summary>
    public class DefaultExceptionHandlerTests
    {
        [Fact]
        public async Task HandleExceptionAsync_Should_Not_Throw()
        {
            // Arrange
            var handler = new DefaultExceptionHandler();
            var context = new Mock<HttpContext>();
            var exception = new Exception("Test exception");

            context.Setup(c => c.TraceIdentifier).Returns("test-trace-id");
            context.Setup(c => c.Request.Path).Returns(new PathString("/test"));
            context.Setup(c => c.Request.Method).Returns("GET");
            context.Setup(c => c.Request.QueryString).Returns(new QueryString());
            context.Setup(c => c.Response.StatusCode).Returns(200);

            // Act & Assert - Não deve lançar exceção
            await handler.HandleExceptionAsync(context.Object, exception);
        }

        [Fact]
        public async Task HandleExceptionAsync_With_NullContext_Should_Throw()
        {
            // Arrange
            var handler = new DefaultExceptionHandler();
            var exception = new Exception("Test exception");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await handler.HandleExceptionAsync(null!, exception));
        }

        [Fact]
        public async Task HandleExceptionAsync_With_NullException_Should_Throw()
        {
            // Arrange
            var handler = new DefaultExceptionHandler();
            var context = new Mock<HttpContext>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await handler.HandleExceptionAsync(context.Object, null!));
        }

        [Fact]
        public async Task HandleExceptionAsync_Should_Handle_Exception_With_All_Properties()
        {
            // Arrange
            var handler = new DefaultExceptionHandler();
            var context = new Mock<HttpContext>();
            var exception = new InvalidOperationException("Test exception");

            context.Setup(c => c.TraceIdentifier).Returns("trace-123");
            context.Setup(c => c.Request.Path).Returns(new PathString("/api/test"));
            context.Setup(c => c.Request.Method).Returns("POST");
            context.Setup(c => c.Request.QueryString).Returns(new QueryString("?param=value"));
            context.Setup(c => c.Response.StatusCode).Returns(500);

            // Act
            await handler.HandleExceptionAsync(context.Object, exception);

            // Assert - Se chegou aqui sem exceção, passou
            Assert.True(true);
        }
    }
}

