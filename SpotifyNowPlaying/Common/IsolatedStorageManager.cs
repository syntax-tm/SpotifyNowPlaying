using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpotifyNowPlaying;

public static class IsolatedStorageManager
{
    private static bool _isInitialized;

    public static void Init()
    {
        if (_isInitialized) return;

        using var store = GetStore();

        store.CreateDirectory(@"playlists");
        store.CreateDirectory(@"users");
        store.CreateDirectory(@"tracks");
        store.CreateDirectory(@"albums");
        store.CreateDirectory(@"misc");
        store.CreateDirectory(@"settings");

        _isInitialized = true;
    }

    public static string GetLocation()
    {
        const string TEST_FILE_NAME = @"test";

        using var store = GetStore();
        using var file = store.CreateFile(TEST_FILE_NAME);

        var fileInfo = new FileInfo(file.Name);
        var directory = fileInfo.Directory?.FullName;

        file.Close();

        store.DeleteFile(TEST_FILE_NAME);

        return directory;
    }

    public static bool FileExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();
        
        return store.FileExists(path);
    }

    public static byte[] ReadFileBytes(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!store.FileExists(path))
        {
            throw new FileNotFoundException($"File '{path}' does not exist.", path);
        }

        using var stream = store.OpenFile(path, FileMode.Open);
        using var sw = new BinaryReader(stream);

        var bytes = new byte[stream.Length];

        for (var i = 0; i < stream.Length; i++)
        {
            bytes[i] = sw.ReadByte();
        }

        return bytes;
    }

    public static string ReadFile(string path, FileCategory category = FileCategory.Default)
    {
        using var store = GetStore();

        if (!store.FileExists(path))
        {
            throw new FileNotFoundException($"File '{path}' does not exist.", path);
        }

        using var stream = store.OpenFile(path, FileMode.Open);
        using var sw = new StreamReader(stream);

        return sw.ReadToEnd();
    }

    public static async Task<string> ReadFileAsync(string path, FileCategory category = FileCategory.Default)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!store.FileExists(path)) throw new FileNotFoundException($"File '{path}' does not exist.", path);

        await using var stream = store.OpenFile(path, FileMode.Open);
        using var sw = new StreamReader(stream);

        return await sw.ReadToEndAsync();
    }

    public static T ReadFile<T>(string path, FileCategory category = FileCategory.Default)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!store.FileExists(path))
        {
            throw new FileNotFoundException($"File '{path}' does not exist.", path);
        }

        using var stream = store.OpenFile(path, FileMode.Open);
        using var sw = new StreamReader(stream);

        var contents = sw.ReadToEnd();
        var obj = JsonConvert.DeserializeObject<T>(contents);

        return obj;
    }

    public static void SaveFile<T>(string path, T contents, FileCategory category = FileCategory.Default, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (store.FileExists(path))
        {
            if (!overwrite) throw new ArgumentException($"File '{path}' exists.", nameof(path));

            store.DeleteFile(path);
        }

        using var stream = store.CreateFile(path);
        using var sw = new StreamWriter(stream);

        var objectJson = JsonConvert.SerializeObject(contents);

        sw.WriteLine(objectJson);
        sw.Flush();
    }

    public static void SaveFile(string path, string contents, FileCategory category = FileCategory.Default, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!overwrite && store.FileExists(path))
        {
            throw new ArgumentException($"File '{path}' exists.", nameof(path));
        }

        using var stream = store.CreateFile(path);
        using var sw = new StreamWriter(stream);

        sw.WriteLine(contents);
        sw.Flush();
    }

    public static async Task SaveFileAsync(string path, string contents, FileCategory category = FileCategory.Default, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!overwrite && store.FileExists(path))
        {
            throw new ArgumentException($"File '{path}' exists.", nameof(path));
        }

        await using var stream = store.CreateFile(path);
        await using var sw = new StreamWriter(stream);
        
        stream.Seek(0, SeekOrigin.End);

        await sw.WriteLineAsync(contents);
        await sw.FlushAsync();
    }

    public static void SaveFile(string path, byte[] contents, FileCategory category = FileCategory.Default, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (store.FileExists(path))
        {
            if (!overwrite) throw new ArgumentException($"File '{path}' exists.", nameof(path));

            store.DeleteFile(path);
        }

        using var stream = store.CreateFile(path);

        stream.Seek(0, SeekOrigin.End);

        stream.Write(contents);
    }
    
    public static async Task SaveFileAsync(string path, byte[] contents, FileCategory category = FileCategory.Default, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        using var store = GetStore();

        if (!overwrite && store.FileExists(path))
        {
            throw new ArgumentException($"File '{path}' exists.", nameof(path));
        }

        await using var stream = store.CreateFile(path);

        stream.Seek(0, SeekOrigin.End);

        await stream.WriteAsync(contents);
    }

    public static IsolatedStorageFile GetStore()
    {
        return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
    }

}
