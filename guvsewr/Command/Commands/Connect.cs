using System.Net.NetworkInformation;

public class Connect : Command
{
    public Connect() : base("cnt", async (cmd) =>
    {
        if (cmd.args.Length == 0)
        {
            Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
            return;
        }

       bool defaultProt;

        if (!cmd.args[0].StartsWith("http://") || !cmd.args[0].StartsWith("https://") 
        || !cmd.args[0].StartsWith("file://") || !cmd.args[0].StartsWith("ftp://")
        || !cmd.args[0].StartsWith("ws://") || !cmd.args[0].StartsWith("wss://")
        || !cmd.args[0].StartsWith("data://") || !cmd.args[0].StartsWith("blob://")
        || !cmd.args[0].StartsWith("about://") || !cmd.args[0].StartsWith("mailto://")
        || !cmd.args[0].StartsWith("tel://") || !cmd.args[0].StartsWith("sms://"))
        {
            defaultProt = true;
        }

        Uri uri;

        if (defaultProt = true)
        { 
            uri = new Uri("https://" + cmd.args[0]);
        }
        else
        {
            uri = new Uri(cmd.args[0]);
        }


        await Connection.Connect(uri);

        Console.Write(Config.root.cli_settings.line_starter + " ");
    })
    { }
}
