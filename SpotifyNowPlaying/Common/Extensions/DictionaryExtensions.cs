using System.Collections.Generic;
using JetBrains.Annotations;

namespace SpotifyNowPlaying;

public static class DictionaryExtensions
{

    public static void Add<T, TKey>([NotNull] this IDictionary<T, TKey> source, [NotNull] IDictionary<T, TKey> values)
    {
        foreach (var key in values.Keys)
        {
            var value = values[key];

            source.Add(key, value);
        }
    }

}
