using System.Reflection;

namespace RayCasting;

internal class EmbeddedResourceManager
{
    public const string SceneObjects = @"SceneObjects.txt";

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
