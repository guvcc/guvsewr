using System.Net.NetworkInformation;

public class PackageCommand : Command
{
    public PackageCommand() : base("pck", async (cmd) =>
    {
        try
        {
            HttpClient client = new HttpClient();

            if (cmd.args.Length > 2)
            {
                Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                return;
            }

            if (cmd.args[0] == "install")
            {
                Package.InstallFromGit(cmd.args[1]);

                Console.WriteLine("Installed! Restart to show effects!");
            }else if (cmd.args[0] == "list")
            {
                string dir = Path.Combine(Directory.GetCurrentDirectory(), "packages");

                if (!Directory.Exists(dir))
                    Console.WriteLine("None!");
                
                foreach (string subdir in Directory.GetDirectories(dir))
                {
                    string packPath = Path.Combine(dir, "package.gpack");

                    Package pack = Package.DeserealizeGPack(File.ReadAllText(packPath));

                    Console.WriteLine($"{pack.name} / {pack.version}");
                }
            }
            else
            {
                Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                return;
            }

            Console.Write(Config.root.cli_settings.line_starter + " ");
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception);
        }
    })
    { }
}
