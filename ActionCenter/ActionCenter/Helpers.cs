using System.Text.RegularExpressions;

namespace ActionCenter
{
    public static class Helpers
    {
        public static string AddSpacesToCamelCase(this string camelCaseString)
        {
            return Regex.Replace(camelCaseString, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }
    }
}
