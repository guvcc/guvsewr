public class GI
{
    public GIExecutor executor;
    public GuvsewrPackage package;
    public Action<GI> Start { get; set; }
    public Action<GI> Stop { get; set; }

    public GI(GIExecutor executor, GuvsewrPackage package)
    {
        this.executor = executor;
        executor.gI = this;
        this.package = package;
        CommandRegistry.Register(new Command(executor.name, executor.executer));
    }
}
public static class GIRegistry
{
    public static Dictionary<string, GI> gis = new Dictionary<string, GI>();

    public static void Register(GI gi)
    {
        gis[gi.executor.name] = gi;
    }
}