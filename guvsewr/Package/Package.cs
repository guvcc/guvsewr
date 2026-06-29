public class Package
{
    public string name, version, mainPath, mainTree;
    public string? configPath;
    public string[] extraPaths;

    public Package(string name, string version, string? configPath, string mainPath, string mainTree, string[] extraPaths)
    {
        this.name = name;
        this.version = version;
        this.configPath = configPath;
        this.mainPath = mainPath;
        this.mainTree = mainTree;
        this.extraPaths = extraPaths;
    }

    public static async void InstallFromGit(string url)
    {
        HttpClient client = new HttpClient();

        string gpack;
        try
        {
            gpack = await client.GetStringAsync(url);
        }
        catch(HttpRequestException exception)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            return;
        }

        Package pack = DeserealizeGPack(gpack);

        try
        {
            string packages = Path.Combine(Directory.GetCurrentDirectory(), "packages");
            if (!Directory.Exists(packages))
                Directory.CreateDirectory(packages);

            string directory = Path.Combine(packages, pack.name);

            Directory.CreateDirectory(directory);

            Console.WriteLine("Installing Gpack...");

            string mainTree = pack.mainTree.Replace("https://github.com/", "https://raw.githubusercontent.com/");
            
            string packagegpack = await client.GetStringAsync(mainTree + "package.gpack");

            File.WriteAllText(Path.Combine(directory, "package.gpack"), packagegpack);

            Console.WriteLine("Done!");

            Console.WriteLine("Installing main class...");

            string mainCS = await client.GetStringAsync(mainTree + pack.mainPath);

            File.WriteAllText(Path.Combine(directory, pack.mainPath), mainCS);

            Console.WriteLine("Done!");

            Console.WriteLine("Installing config json...");

            string config = await client.GetStringAsync(mainTree + pack.configPath);

            File.WriteAllText(Path.Combine(directory, pack.configPath), config);

            Console.WriteLine("Done!");

            Console.WriteLine("Installing extra files...");

            foreach (string path in pack.extraPaths)
            {
                string extra = await client.GetStringAsync(mainTree + path);

                File.WriteAllText(Path.Combine(directory, path), extra);
            }

            Console.WriteLine("Done!");

            Console.WriteLine("Compiling Scripts...");

            CompileScripts.Compile(directory);

            Console.WriteLine("Done!");
        }
        catch(HttpRequestException exception)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            return;
        }
    }

    public static Package DeserealizeGPack(string gpack)
    {
        Package pack = new Package(null ,null, null, null, null, null);
        using StringReader reader = new StringReader(gpack);

        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("name"))
            {
                pack.name = line.Split("=")[1];
            }
            else if (line.StartsWith("version"))
            {
                pack.version = line.Split("=")[1];
            }
            else if (line.StartsWith("main"))
            {
                pack.mainPath = line.Split("=")[1];
            }
            else if (line.StartsWith("config"))
            {
                pack.configPath = line.Split("=")[1];
            }
            else if (line.StartsWith("extrafiles"))
            {
                pack.extraPaths = line.Split("=")[1].Split(",").ToArray();
            }
            else if (line.StartsWith("maintree"))
            {
                pack.mainTree = line.Split("=")[1];
            }
        }

        return pack;
    }
}