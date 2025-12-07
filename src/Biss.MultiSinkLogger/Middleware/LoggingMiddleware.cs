using Biss.MultiSinkLogger.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using System.Buffers;
using System.Diagnostics;
using Biss.MultiSinkLogger.Constants;
using Biss.MultiSinkLogger.Security;

namespace Biss.MultiSinkLogger.Middleware
{
    /// <summary>
    /// Middleware responsável por capturar e logar informações de requisição e resposta HTTP.
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggingMiddlewareSettings _settings;
        private static readonly ArrayPool<char> _charPool = ArrayPool<char>.Shared;

        /// <summary>
        /// Inicializa o middleware com o próximo delegado na pipeline e configurações.
        /// </summary>
        /// <param name="next">Próximo delegado na pipeline.</param>
        /// <param name="settings">Configurações do middleware (opcional).</param>
        public LoggingMiddleware(RequestDelegate next, IOptions<LoggingMiddlewareSettings>? settings = null)
        {
            _next = next;
            _settings = settings?.Value ?? new LoggingMiddlewareSettings();
        }

        /// <summary>
        ///  Inicializa o middleware com o próximo delegado na pipeline.
        ///  </summary>
        ///  <param name="next">Próximo delegado na pipeline.</param>
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _settings = new LoggingMiddlewareSettings(); // Inicializar com valores padrão
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

                // Filtrar dados sensíveis se habilitado
                var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                var filteredHeaders = _settings.FilterSensitiveData 
                    ? SensitiveDataFilter.FilterHeaders(headers) 
                    : headers;
                
                var filteredBody = _settings.FilterSensitiveData 
                    ? SensitiveDataFilter.FilterSensitiveData(requestBody) 
                    : requestBody;
                
                var filteredQueryString = _settings.FilterSensitiveData 
                    ? SensitiveDataFilter.FilterSensitiveData(context.Request.QueryString.ToString()) 
                    : context.Request.QueryString.ToString();

                var requestLog = new
                {
                    TraceId = context.TraceIdentifier,
                    context.Request.Scheme,
                    context.Request.Host,
                    context.Request.Path,
                    QueryString = filteredQueryString,
                    Headers = filteredHeaders,
                    Body = filteredBody
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

                    // Filtrar dados sensíveis na resposta se habilitado
                    var responseHeaders = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                    var filteredResponseHeaders = _settings.FilterSensitiveData 
                        ? SensitiveDataFilter.FilterHeaders(responseHeaders) 
                        : responseHeaders;
                    
                    var filteredResponseBody = _settings.FilterSensitiveData 
                        ? SensitiveDataFilter.FilterSensitiveData(responseText) 
                        : responseText;

                    var responseLog = new
                    {
                        TraceId = context.TraceIdentifier,
                        context.Response.StatusCode,
                        Headers = filteredResponseHeaders,
                        Body = filteredResponseBody,
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
        /// Usa ArrayPool para evitar alocações desnecessárias e memory leaks.
        /// </summary>
        /// <param name="bodyStream">O stream do corpo da requisição.</param>
        /// <returns>O conteúdo do corpo como string, truncado se necessário.</returns>
        private async Task<string> ReadAndTruncateAsync(Stream bodyStream)
        {
            // Garantir que MaxBodyLength é válido (validação de segurança adicional)
            var maxReadSize = Math.Max(1, _settings.MaxBodyLength); // Garantir pelo menos 1
            var buffer = _charPool.Rent(maxReadSize);

            try
            {
                using var reader = new StreamReader(bodyStream, leaveOpen: true);
                var readSize = await reader.ReadBlockAsync(buffer, 0, maxReadSize);

                bodyStream.Position = 0; // Resetar o stream após a leitura
                var bodyContent = new string(buffer, 0, readSize);

                return bodyContent.Length >= maxReadSize 
                    ? $"{bodyContent}... [truncated]" 
                    : bodyContent;
            }
            finally
            {
                // Sempre retornar o buffer ao pool para evitar memory leaks
                _charPool.Return(buffer);
            }
        }
    }
}
