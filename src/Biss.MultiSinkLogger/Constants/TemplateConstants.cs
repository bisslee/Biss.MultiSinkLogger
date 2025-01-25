namespace Biss.MultiSinkLogger.Constants
{
    public static class TemplateConstants
    {
        public const string OUTPUT_TEMPLATE = "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}";
    }
}
