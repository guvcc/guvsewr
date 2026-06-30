using System.Reflection;

public class GuvsewrPackage
{
    public static List<GuvsewrPackage> Packages { get; } = new();

    public Package manifest { get; set; }
    public string basePath { get; set; }
    public Assembly Assembly { get; set; }
    public LoggerAPI log { get; set; }
    public CommandAPI command { get; set; }
    public FileAPI file { get; set; }
    public JsonAPI json { get; set; }

    public virtual void Start() { }

    public GuvsewrPackage(Package manifest, string basePath, Assembly assembly)
    {
        this.manifest = manifest;
        this.basePath = basePath;
        this.Assembly = assembly;

        log = new LoggerAPI();
        command = new CommandAPI();
        file = new FileAPI();
        json = new JsonAPI();

        log.package = this;
        command.package = this;
        file.package = this;
        json.package = this;

        Start();
    }
}