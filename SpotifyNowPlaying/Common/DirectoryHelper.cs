using System;
using System.IO;
using System.Security;
using JetBrains.Annotations;

namespace SpotifyNowPlaying
{
    public static class DirectoryHelper
    {
        public static void EnsureDirectoryExists([NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (!IsValidPath(path)) throw new ArgumentException($"Directory path '{path}' is not valid.");

            if (Directory.Exists(path)) return;

            Directory.CreateDirectory(path);
        }

        public static bool IsValidPath(string path)
        {
            return TryGetFullPath(path, out var _);
        }

        public static bool TryGetFullPath(string path, out string result)
        {
            result = string.Empty;
            if (string.IsNullOrWhiteSpace(path)) { return false; }
            var status = false;

            try
            {
                result = Path.GetFullPath(path);
                status = true;
            }
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (NotSupportedException) { }
            catch (PathTooLongException) { }

            return status;
        }

    }
}
