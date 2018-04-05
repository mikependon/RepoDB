using System.Collections.Generic;

namespace RepoDb.Extensions
{
    public static class StringExtension
    {
        internal static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }
    }
}
