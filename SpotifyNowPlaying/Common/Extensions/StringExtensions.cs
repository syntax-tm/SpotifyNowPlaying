using System.Globalization;
using JetBrains.Annotations;

namespace SpotifyNowPlaying.Extensions
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
    }
}
