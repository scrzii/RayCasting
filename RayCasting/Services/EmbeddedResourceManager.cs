using System.Reflection;

namespace RayCasting.Services;

internal class EmbeddedResourceManager
{
    public static string SceneObjects 
    {
        get => GetFileData(@"SceneObjects.txt");
    }

    public static string GetFileData(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(x => x.EndsWith(filename));
        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new Exception($"Resource stream of file {resourceName} does not found");
        }

        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
