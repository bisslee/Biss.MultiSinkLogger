namespace Biss.MultiSinkLogger.Http
{

    public static class HttpContentExtensions
    {
        public static async Task<string> ReadAsStringSafeAsync(this HttpContent content)
        {
            return content != null ? await content.ReadAsStringAsync() : string.Empty;
        }
    }
}
