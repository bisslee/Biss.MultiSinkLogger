namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class FileSinkSettings : ISinkSettings
    {
        public string Path { get; set; } = null!;
        public string Filename { get; set; } = null!;
        public string RollingInterval { get; set; } = "Day";
        public int RetainedFileCountLimit { get; set; } = 30;
        public bool Shared { get; set; } = false;
    }
}
