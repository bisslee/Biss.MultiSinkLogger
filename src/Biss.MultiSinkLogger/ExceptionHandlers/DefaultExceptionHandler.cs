using Biss.MultiSinkLogger.Constants;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace Biss.MultiSinkLogger.ExceptionHandlers
{
    /// <summary>
    /// Tratamento padrão de exceções com logging estruturado.
    /// </summary>
    public class DefaultExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Trata a exceção registrando informações estruturadas no log.
        /// </summary>
        /// <param name="context">Contexto da requisição HTTP.</param>
        /// <param name="exception">Exceção a ser tratada.</param>
        /// <returns>Task que representa a operação assíncrona.</returns>
        public Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            try
            {
                // Adicionar propriedades ao contexto de log para melhor rastreabilidade
                using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
                using (LogContext.PushProperty("Path", context.Request.Path))
                using (LogContext.PushProperty("Method", context.Request.Method))
                using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
                using (LogContext.PushProperty("QueryString", context.Request.QueryString.ToString()))
                {
                    // Log estruturado com todas as informações relevantes
                    Log.Error(
                        exception,
                        LogMessages.UnhandledException + " TraceId: {TraceId}, Path: {Path}, Method: {Method}",
                        context.TraceIdentifier,
                        context.Request.Path,
                        context.Request.Method);
                }
            }
            catch (Exception logException)
            {
                // Se falhar ao logar, pelo menos tentar logar a falha original
                // sem usar LogContext para evitar problemas em cascata
                Log.Error(
                    logException,
                    "Falha ao registrar exceção no handler. Exceção original: {OriginalException}",
                    exception.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
