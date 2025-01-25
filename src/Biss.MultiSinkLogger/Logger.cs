using Serilog;

namespace Biss.MultiSinkLogger
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Log.Information(message);
        }

        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                Log.Error(message);
            }
            else
            {
                Log.Error(ex, message);
            }
        }

        public static void Warning(string message)
        {
            Log.Warning(message);
        }

        public static void Fatal(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                Log.Fatal(message);
            }
            else
            {
                Log.Fatal(ex, message);
            }
        }
    }
}
