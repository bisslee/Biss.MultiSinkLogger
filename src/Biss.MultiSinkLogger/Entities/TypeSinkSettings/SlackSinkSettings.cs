using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biss.MultiSinkLogger.Entities.TypeSinkSettings
{
    public class SlackSinkSettings : ISinkSettings
    {
        public string WebhookUrl { get; set; } = null!;
        public string Channel { get; set; } = "#general";
        public string Username { get; set; } = "Logger";
        public string IconEmoji { get; set; } = ":robot_face:"; // Opcional
    }
}
