//Always give unique names to classes so there are no missmatches!
using System.Text.Json;
public class Example : Command
{
    public Example() : base("exp", (cmd) =>
    {
        JsonDocument doc = JsonDocument.Parse(File.ReadAllText("config.json"));
        Console.WriteLine(doc.RootElement.GetProperty("Message").GetString());
    })
    { }
}