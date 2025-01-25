using Biss.MultiSinkLogger.Constants;

namespace Biss.MultiSinkLogger.Entities
{
    public class LoggerManagerSettings
    {
        public string MinimumLevel { get; set; } = "Information";
        public string OutputTemplate { get; set; } = TemplateConstants.OUTPUT_TEMPLATE;
        public List<Sink> Sinks { get; set; } = new List<Sink>();
    }
}

