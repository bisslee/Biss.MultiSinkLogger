namespace Biss.MultiSinkLogger.Configuration
{
    /// <summary>
    /// Configurações para o middleware de logging.
    /// </summary>
    public class LoggingMiddlewareSettings
    {
        /// <summary>
        /// Tamanho máximo do corpo da requisição/resposta a ser logado (em caracteres).
        /// </summary>
        public int MaxBodyLength { get; set; } = 1000;

        /// <summary>
        /// Indica se deve filtrar dados sensíveis dos logs.
        /// </summary>
        public bool FilterSensitiveData { get; set; } = true;
    }
}

