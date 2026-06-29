using System.Net.NetworkInformation;

public class HttpGet : Command
{
    public HttpGet() : base("htgt", async (cmd) =>
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

                    Console.WriteLine($@"{(int) response.StatusCode} {response.StatusCode}");
                }
                catch(HttpRequestException exception)
                {
                     Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
                     return;
                }
            }else if (cmd.args[0] == "html")
            {
                try
                {
                    Console.WriteLine(client.GetStringAsync(cmd.args[1]).Result);
                }
                catch(HttpRequestException exception)
                {
                     Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
                     return;
                }
            }else
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
