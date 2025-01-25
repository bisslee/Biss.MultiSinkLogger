using Serilog;
using Biss.MultiSinkLogger.Constants;

namespace Biss.MultiSinkLogger.Http
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestContent;
            try
            {
                requestContent = await request.Content.ReadAsStringSafeAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, LogMessages.ErrorReadingRequestContent);
                requestContent = LogMessages.ErrorReadingRequestContent;
            }

            Log.Information(LogMessages.SendingHttpRequest, request.Method, request.RequestUri, requestContent);

            var response = await base.SendAsync(request, cancellationToken);

            string responseContent;
            try
            {
                responseContent = await response.Content.ReadAsStringSafeAsync();
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
