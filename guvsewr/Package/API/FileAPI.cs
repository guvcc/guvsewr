public class FileAPI
{
    public GuvsewrPackage package { get; set; }

    public void WriteAllText(string path, string text)
    {
        File.WriteAllText(path, text);
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }

    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}