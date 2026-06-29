public class Package
{
    public string name, version, mainPath;
    public string? configPath;
    public string[] extraPaths;

    public Package(string name, string version, string? configPath, string mainPath, string mainTree, string[] extraPaths)
    {
        this.name = name;
        this.version = version;
        this.configPath = configPath;
        this.mainPath = mainPath;
        this.extraPaths = extraPaths;
    }

    public static async Task InstallFromGit(string url)
    {
        HttpClient client = new HttpClient();

        string gpack;
        try
        {
            gpack = await client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            Console.WriteLine(ex.ToString());
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

            string packagegpack = await client.GetStringAsync(url);

            File.WriteAllText(Path.Combine(directory, "package.gpack"), packagegpack);

            Console.WriteLine("Done!");

            Console.WriteLine("Installing main class...");

            string mainCS = await client.GetStringAsync(pack.mainPath);

            File.WriteAllText(Path.Combine(directory, pack.mainPath), mainCS);

            Console.WriteLine("Done!");


            if (!string.IsNullOrWhiteSpace(pack.configPath))
            {
                Console.WriteLine("Installing config json...");

                string config = await client.GetStringAsync(pack.configPath);

                File.WriteAllText(Path.Combine(directory, pack.configPath), config);

                Console.WriteLine("Done!");
            }

            Console.WriteLine("Installing extra files...");

            foreach (string path in pack.extraPaths)
            {
                string extra = await client.GetStringAsync(path);

                File.WriteAllText(Path.Combine(directory, path), extra);
            }

            Console.WriteLine("Done!");

            Console.WriteLine("Compiling Scripts...");

            CompileScripts.Compile(directory);

            Console.WriteLine("Done!");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            Console.WriteLine(ex.ToString());
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
        }

        return pack;
    }
}