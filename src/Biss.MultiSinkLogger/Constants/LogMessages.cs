namespace Biss.MultiSinkLogger.Constants
{
    public static class LogMessages
    {
        // HttpLoggingHandler
        public const string SendingHttpRequest = "Enviando requisição HTTP: {Method} {Uri} - Body: {Body}";
        public const string ReceivedHttpResponse = "Resposta HTTP recebida: {StatusCode} - Body: {Body}";
        public const string ErrorReadingRequestContent = "Erro ao ler o conteúdo da requisição";
        public const string ErrorReadingResponseContent = "Erro ao ler o conteúdo da resposta";

        // Outros locais onde você pode usar constantes
        // Middleware de Logging
        public const string HandlingRequest = "Processando requisição: {@Request}";
        public const string FinishedHandlingRequest = "Requisição processada: {@Response}";

        // Middleware de Exceção
        public const string UnhandledException = "Uma exceção não tratada ocorreu ao processar a requisição.";
        public const string ErrorHandlingRequest = "Erro ao processar a requisição.";

        // Controladores
        public const string WeatherForecastEndpointAccessed = "Endpoint WeatherForecast GET acessado";
        public const string ExternalApiDataRetrieved = "Dados da API externa recuperados com sucesso.";
    }
}
