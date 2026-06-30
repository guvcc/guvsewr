using System.Text;
using System.Diagnostics;

public class Package
{
    public string name, version, mainPath;
    public string? configPath;
    public string[] extraPaths;
    public string csprojPath;
    public Package(string name, string version, string? configPath, string mainPath, string mainTree, string[] extraPaths, string csprojPath)
    {
        this.name = name;
        this.version = version;
        this.configPath = configPath;
        this.mainPath = mainPath;
        this.extraPaths = extraPaths;
        this.csprojPath = csprojPath;
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

            File.WriteAllText(Path.Combine(directory, Path.GetFileName(new Uri(pack.mainPath).AbsolutePath)), mainCS);

            Console.WriteLine("Done!");

            if (!string.IsNullOrWhiteSpace(pack.configPath))
            {
                Console.WriteLine("Installing config json...");

                string config = await client.GetStringAsync(pack.configPath);

                File.WriteAllText(Path.Combine(directory, Path.GetFileName(new Uri(pack.configPath).AbsolutePath)), config);

                Console.WriteLine("Done!");
            }

            Console.WriteLine("Installing extra files...");

            foreach (string path in pack.extraPaths)
            {
                string extra = await client.GetStringAsync(path);

                File.WriteAllText(Path.Combine(directory, Path.GetFileName(new Uri(path).AbsolutePath)), extra);
            }

            Console.WriteLine("Done!");

            Console.WriteLine("Downloading Csproj!");

            string csproj = await client.GetStringAsync(pack.csprojPath);

            File.WriteAllText(Path.Combine(directory, Path.GetFileName(new Uri(pack.csprojPath).AbsolutePath)), csproj);

            Console.WriteLine("Done!");

            Console.WriteLine("Downloading Packages!");

            await DownloadPackages(directory);

            Console.WriteLine("Done!");

            Console.WriteLine("Compiling Scripts...");

            CompileScripts.Compile(directory, pack);

            Console.WriteLine("Done!");

            Console.WriteLine("Installation Complete!");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            Console.WriteLine(ex.ToString());
            return;
        }
    }

    public static async Task DownloadPackages(string dir)
    {
        Process process = new Process();

        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"restore \"{Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories).First()}\" --packages \"{Path.Combine(dir, ".nuget")}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine(output);

        if (process.ExitCode != 0)
        {
            Console.WriteLine(error);
        }
    }

    public static Package DeserealizeGPack(string gpack)
    {
        Package pack = new Package(null, null, null, null, null, null, null);
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
            else if (line.StartsWith("csproj"))
            {
                pack.csprojPath = line.Split("=")[1];
            }
        }

        return pack;
    }
}