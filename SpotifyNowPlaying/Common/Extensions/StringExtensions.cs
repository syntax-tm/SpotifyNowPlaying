using System;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace SpotifyNowPlaying.Common
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase([NotNull] this string a, [NotNull] string b)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            return currentCulture.CompareInfo.Compare(a, b, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool ContainsIgnoreCase([NotNull] this string a, [NotNull] string b)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            return currentCulture.CompareInfo.IndexOf(a, b, CompareOptions.IgnoreCase) >= 0;
        }

        public static ILookup<string, string> SplitQueryParams([NotNull] this string query)
        {
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));

            var responseParams = query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .Where(p => p.Length == 2)
                .ToLookup(k => k[0], k => Uri.UnescapeDataString(k[1]), StringComparer.OrdinalIgnoreCase);

            return responseParams;
        }
    }
}
