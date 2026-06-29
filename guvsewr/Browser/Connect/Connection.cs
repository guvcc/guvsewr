public class Connection
{
    public static async Task<Page> Connect(Uri url )
    {
        Console.WriteLine("Connecting...");
        HttpClient client = new HttpClient();

        HttpResponseMessage response;
        string html;
        try
        {
            response = await client.GetAsync(url);

            html = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Done!");
            Console.WriteLine("If nothing happens then you do not have a GI installed");
            Console.WriteLine("Install a GI with > pck install //NAME//");
        }
        catch(HttpRequestException exception)
        {
            Console.WriteLine(Config.root.cli_settings.errors["host_not_found"]);
            return null;
        }

        if (response == null)
            return null;

        return new Page(
            response.RequestMessage!.RequestUri!,
            html,
            (int)response.StatusCode);
        
    }
}