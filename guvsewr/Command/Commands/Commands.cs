using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
public static class Commands
{
    public static void Register()
    {
        CommandRegistry.Register(new Command("clear", (cmd) =>
        {
            Console.Clear();
        }));

        CommandRegistry.Register(new Command("cnt", static async (cmd) =>
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
        }));

        CommandRegistry.Register(new Command("htgt", async (cmd) =>
        {
            try
            {
                HttpClient client = new HttpClient();

                if (cmd.args.Length > 2)
                {
                    Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                    return;
                }

                if (cmd.args[0] == "code")
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(cmd.args[1]);

                        Console.WriteLine($@"{(int)response.StatusCode} {response.StatusCode}");
                    }
                    catch (HttpRequestException exception)
                    {
                        Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
                        return;
                    }
                }
                else if (cmd.args[0] == "html")
                {
                    try
                    {
                        Console.WriteLine(client.GetStringAsync(cmd.args[1]).Result);
                    }
                    catch (HttpRequestException exception)
                    {
                        Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                    return;
                }

                Console.Write(Config.root.cli_settings.line_starter + " ");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }));

        CommandRegistry.Register(new Command("pck", async (cmd) =>
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
                    await Package.InstallFromGit(cmd.args[1]);
                }
                else if (cmd.args[0] == "list")
                {
                    foreach (var pack in GuvsewrPackage.Packages)
                    {
                        Console.WriteLine($"{pack.manifest.name} / {pack.manifest.version}");
                    }
                }
                else
                {
                    Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                    return;
                }

                Console.Write(Config.root.cli_settings.line_starter + " ");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }));

        CommandRegistry.Register(new Command("ping", (cmd) =>
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
            }
            else
            {
                Console.WriteLine(reply.Status);
            }
        }));

        CommandRegistry.Register(new Command("fetch", (cmd) =>
        {
            string[] art = Config.GetVariable("@ascii_art")?.Replace("\r", "").Split('\n', StringSplitOptions.None) ?? Array.Empty<string>();

            string[] info = new string[]
            {
                $"Name: {Config.root.distro_settings.name}",
                $"Version: {Config.root.distro_settings.version}",
                $"Author: {Config.root.distro_settings.author}",
                $"Description: {Config.root.distro_settings.description}"
            };



            List<string> wrapped = new();

            string Clean(string s) => Regex.Replace(s, @"\x1B\[[0-9;]*m", "");

            int leftColumnWidth = art.Max(l => Clean(l).Length) + 4;
            int infoWidth = 40;

            foreach (string line in info)
            {
                string remaining = line;

                while (Clean(remaining).Length > infoWidth)
                {
                    int split = remaining.LastIndexOf(' ', infoWidth);

                    if (split == -1)
                        split = infoWidth;

                    wrapped.Add(remaining[..split]);
                    remaining = remaining[(split + 1)..];
                }

                wrapped.Add(remaining);
            }

            int i = 0;

            while (i < art.Length || i < wrapped.Count)
            {
                string artLine = i < art.Length ? art[i] : "";
                string infoLine = i < wrapped.Count ? wrapped[i] : "";

                int pad = leftColumnWidth - Clean(artLine).Length;
                if (pad < 1) pad = 1;

                Console.Write(artLine);
                Console.Write(new string(' ', pad));
                Console.WriteLine(infoLine);

                i++;
            }
        }));
    }
}
