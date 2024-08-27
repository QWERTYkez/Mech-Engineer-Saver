using System.IO;
using System.Security.Cryptography;

namespace MechEngineerSaver;

public static class Extension
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> Action)
    {
        foreach (T item in collection)
            Action(item);
        return collection;
    }

    public static T SetAct<T>(this T item, Action<T> Action)
    {
        Action(item);
        return item;
    }


    static MD5 MD5 { get; } = MD5.Create();
    public static string GetMD5Hash(this FileInfo file) => string.Join('.', MD5.ComputeHash(file.ReadAllBytes()));

    public static byte[] ReadAllBytes(this FileInfo file) => File.ReadAllBytes(file.FullName);
    public static string ReadAllText(this FileInfo file) => File.ReadAllText(file.FullName);
    public static void WriteAllText(this FileInfo file, string Text) => File.WriteAllText(file.FullName, Text);
    public static StreamReader OpenText(this FileInfo file) => File.OpenText(file.FullName);
}
