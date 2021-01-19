namespace ImageClassification.API.Extensions
{
    public static class StringExtensions
    {
        public static string TrimStart(this string target, string trimString, bool useMultipleTimes = false)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
                if (!useMultipleTimes)
                    break;
            }

            return result;
        }

        public static string TrimEnd(this string target, string trimString, bool useMultipleTimes = false)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
                if (!useMultipleTimes)
                    break;
            }

            return result;
        }
    }
}
