//Always give unique names to classes so there are no missmatches!
using System.Reflection;

public class Example : GuvsewrPackage
{
    public Example(Package manifest, string basePath, Assembly assembly) : base(manifest, basePath, assembly)
    {
    }

    public override void Start()
    {
        command.Register(new Command("exp", (cmd) =>
        {
            log.WriteLine(json.Read("config.json")["message"].ToString());
        }));
    }
}