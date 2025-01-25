using Microsoft.AspNetCore.Http;

namespace Biss.MultiSinkLogger.ExceptionHandlers
{

    /// <summary>
    /// Tratamento padrão de exceções
    /// </summary>
    public class DefaultExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// trata a exceção
        /// </summary>
        /// <param name="context">Contexto da requisição</param>
        /// <param name="exception">Exceção a ser tratada</param>
        /// <returns>Task</returns>
        public Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
