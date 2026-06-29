using System.Net.NetworkInformation;

public class PingCommand : Command
{
    public PingCommand() : base("ping", (cmd) =>
    {
        if (cmd.args.Length == 0)
        {
            Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
            return;
        }

        Ping ping = new Ping();
        PingReply reply = ping.Send(cmd.args[0]);
        if (reply.Status == IPStatus.Success)
        {
            Console.WriteLine($"Reply from {reply.Address}: {reply.RoundtripTime} ms");
        }else
        {
            Console.WriteLine(reply.Status);
        }
    })
    { }
}
