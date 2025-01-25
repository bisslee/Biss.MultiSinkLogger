namespace Biss.MultiSinkLogger.Entities
{
    public static class EnumParse
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
