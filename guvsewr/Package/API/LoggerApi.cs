public class LoggerAPI
{
    public GuvsewrPackage package { get; set; }

    public void Write(string text)
    {
        Console.Write(text);
    }

    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }
}