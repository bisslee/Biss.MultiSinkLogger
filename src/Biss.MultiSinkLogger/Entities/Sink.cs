using Biss.MultiSinkLogger.Entities.TypeSinkSettings;

namespace Biss.MultiSinkLogger.Entities
{
    public class Sink
    {
        public string Type { get; set; } = null!;
        public ISinkSettings Settings { get; set; } = null!;
        public bool Active { get; set; } = true;
    }
}
