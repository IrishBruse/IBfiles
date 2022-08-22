namespace IBfiles.Utilities;

using System.Reflection;

using IBfiles.ApplicationBackend;

public static class ResourceLoader
{
    public static byte[] GetEmbeddedResourceBytes(string fileLocation)
    {
        Assembly assembly = typeof(Application).Assembly;
        string name = assembly.FullName.Split(',')[0] + "." + fileLocation.Replace('/', '.');
        using Stream s = assembly.GetManifestResourceStream(name);
        byte[] ret = new byte[s.Length];
        _ = s.Read(ret, 0, (int)s.Length);
        return ret;
    }
}
