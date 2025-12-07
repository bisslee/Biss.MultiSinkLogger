using Serilog;
using Biss.MultiSinkLogger.Constants;
using Biss.MultiSinkLogger.Security;

namespace Biss.MultiSinkLogger.Http
{
    /// <summary>
    /// Handler HTTP para logging de requisições e respostas com filtro de dados sensíveis.
    /// </summary>
    public class HttpLoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestContent;
            try
            {
                var rawContent = request.Content != null 
                    ? await request.Content.ReadAsStringSafeAsync() 
                    : string.Empty;
                requestContent = SensitiveDataFilter.FilterSensitiveData(rawContent);
            }
            catch (Exception ex)
            {
                Log.Error(ex, LogMessages.ErrorReadingRequestContent);
                requestContent = LogMessages.ErrorReadingRequestContent;
            }

            // Filtrar URI se contiver dados sensíveis
            var filteredUri = SensitiveDataFilter.FilterSensitiveData(request.RequestUri?.ToString() ?? string.Empty);

            Log.Information(LogMessages.SendingHttpRequest, request.Method, filteredUri, requestContent);

            var response = await base.SendAsync(request, cancellationToken);

            string responseContent;
            try
            {
                var rawResponseContent = response.Content != null 
                    ? await response.Content.ReadAsStringSafeAsync() 
                    : string.Empty;
                responseContent = SensitiveDataFilter.FilterSensitiveData(rawResponseContent);
            }
            catch (Exception ex)
            {
                Log.Error(ex, LogMessages.ErrorReadingResponseContent);
                responseContent = LogMessages.ErrorReadingResponseContent;
            }

            Log.Information(LogMessages.ReceivedHttpResponse, response.StatusCode, responseContent);

            return response;
        }
    }
}
