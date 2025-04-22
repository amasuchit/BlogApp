using System.Text.RegularExpressions;

namespace WeDoBlog.Helpers
{
    public static class RemoveHtmlTagHelper
    {
        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            // Use a regular expression to remove HTML tags
            return Regex.Replace(input, "<.*?>|&.*?;", string.Empty);
        }
    }
}
