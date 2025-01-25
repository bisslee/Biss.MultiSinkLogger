using Biss.MultiSinkLogger.Constants;
using Biss.MultiSinkLogger.ExceptionHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.Json;

namespace Biss.MultiSinkLogger.Middleware
{
    /// <summary>
    /// Middleware responsável por capturar e logar exceções não tratadas na aplicação.
    /// </summary>
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly string _defaultErrorMessage;

        /// <summary>
        /// Inicializa o middleware com o próximo delegado e um handler de exceções.
        /// </summary>
        /// <param name="next">O próximo middleware a ser executado.</param>
        /// <param name="exceptionHandler">O handler de exceções para registrar e tratar erros.</param>
        /// <param name="configuration">As configurações da aplicação para mensagens personalizadas.</param>

        public ExceptionLoggingMiddleware(RequestDelegate next, IExceptionHandler exceptionHandler, IConfiguration configuration)
        {
            _next = next;
            _exceptionHandler = exceptionHandler;

            // Busca a mensagem personalizada do JSON de configuração ou usa um padrão
            _defaultErrorMessage = configuration["ErrorHandling:DefaultErrorMessage"]
                                   ?? "Ocorreu um erro inesperado.";
        }

        /// <summary>
        /// Invoca o middleware para capturar exceções e enviar uma resposta padronizada.
        /// </summary>
        /// <param name="context">O contexto HTTP atual.</param>
        /// <returns>Uma tarefa que representa a execução assíncrona do middleware.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, LogMessages.UnhandledException);

                // Usar o handler para registrar a exceção
                await _exceptionHandler.HandleExceptionAsync(context, ex);

                // Retornar uma resposta de erro personalizada
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    error = _defaultErrorMessage
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
