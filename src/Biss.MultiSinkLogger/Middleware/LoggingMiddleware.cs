using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using Biss.MultiSinkLogger.Constants;

namespace Biss.MultiSinkLogger.Middleware
{

    /// <summary>
    /// Middleware responsável por capturar e logar informações de requisição e resposta HTTP.
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private const int MaxBodyLength = 1000;

        /// <summary>
        ///  Inicializa o middleware com o próximo delegado na pipeline.
        ///  </summary>
        ///  <param name="next">Próximo delegado na pipeline.</param>
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método invocado pelo ASP.NET Core para processar a requisição HTTP.
        /// </summary>
        /// <param name="context">O contexto HTTP atual.</param>
        /// <returns>Uma tarefa que representa a execução assíncrona do middleware.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Captura o corpo da requisição
                context.Request.EnableBuffering();
                var requestBody = await ReadAndTruncateAsync(context.Request.Body);

                var requestLog = new
                {
                    TraceId = context.TraceIdentifier,
                    context.Request.Scheme,
                    context.Request.Host,
                    context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    Body = requestBody
                };

                Log.Information(LogMessages.HandlingRequest, requestLog);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to log request details.");
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseText = await ReadAndTruncateAsync(context.Response.Body);

                    var responseLog = new
                    {
                        TraceId = context.TraceIdentifier,
                        context.Response.StatusCode,
                        Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                        Body = responseText,
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                    };

                    Log.Information(LogMessages.FinishedHandlingRequest, responseLog);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to log response details.");
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        /// <summary>
        /// Lê o corpo da requisição e o trunca se for maior que o tamanho máximo permitido.
        /// </summary>
        /// <param name="bodyStream">O stream do corpo da requisição.</param>
        /// <returns>O conteúdo do corpo como string, truncado se necessário.</returns>
        private async Task<string> ReadAndTruncateAsync(Stream bodyStream)
        {
            const int maxReadSize = MaxBodyLength;
            var buffer = new char[maxReadSize];
            using var reader = new StreamReader(bodyStream, leaveOpen: true);
            var readSize = await reader.ReadBlockAsync(buffer, 0, maxReadSize);

            bodyStream.Position = 0; // Resetar o stream após a leitura
            var bodyContent = new string(buffer, 0, readSize);

            return bodyContent.Length >= maxReadSize ? $"{bodyContent}... [truncated]" : bodyContent;
        }
    }
}
