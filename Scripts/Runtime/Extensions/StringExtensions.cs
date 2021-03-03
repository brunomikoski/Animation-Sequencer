namespace BrunoMikoski.AnimationSequencer
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Length <= maxLength ? value : $"{value.Substring(0, maxLength)}{truncateString}";
        }
    }
}
