using System.Text.Json;
using System.Text.Json.Nodes;

public class JsonAPI
{
    public GuvsewrPackage package { get; set; }

    public JsonNode Read(string path)
    {
        return JsonNode.Parse(File.ReadAllText(path))!;
    }

    public void Write<T>(string path, T value)
    {
        string json = JsonSerializer.Serialize(
            value,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(path, json);
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}