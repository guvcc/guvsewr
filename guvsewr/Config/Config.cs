using System.Text.Json;
public class Config
{
    public static Root root = ConfDeserializer.DeserializeJson(File.ReadAllText("conf/config.json"));
}
public class ConfDeserializer
{
    public static Root DeserializeJson(string json)
    {
        Root preLoaded = JsonSerializer.Deserialize<Root>(json);

        foreach (AppVariable appVariable in preLoaded.app_variables)
        {
            if (appVariable.value.StartsWith("@load_from_file:"))
            {
                string filePath = appVariable.value.Substring("@load_from_file:".Length);
                if (File.Exists(filePath))
                {
                    appVariable.value = File.ReadAllText(filePath);
                }
                else
                {
                    Console.WriteLine($"Error: File '{filePath}' not found.");
                    appVariable.value = "NULL";
                }
            }
        }

        for (int i = 0; i < preLoaded.introduction_sentences.Count; i++)
        {
            string sentence = preLoaded.introduction_sentences[i];

            foreach (AppVariable appVariable in preLoaded.app_variables)
            {
                preLoaded.introduction_sentences[i] = sentence.Replace($"{appVariable.id}", appVariable.value);
            }
        }

        return preLoaded;
    }
}

public class Root
{
    public DistroSettings distro_settings { get; set; }
    public List<string> introduction_sentences { get; set; }
    public CLISettings cli_settings { get; set; }
    public List<AppVariable> app_variables { get; set; }
}

public class DistroSettings
{
    public string name { get; set; }
    public string version { get; set; }
    public string author { get; set; }
    public string description { get; set; }
}

public class CLISettings
{
    public string line_starter { get; set; }
    public Dictionary<string, string> errors { get; set; }
}

public class AppVariable
{
    public string id { get; set; }
    public string value { get; set; }
}
