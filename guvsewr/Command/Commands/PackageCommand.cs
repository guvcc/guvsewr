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
                
            }else if (cmd.args[0] == "list")
            {
                
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
