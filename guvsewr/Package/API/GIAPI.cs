public class GIAPI
{
    public GuvsewrPackage package { get; set; }

    public void Register(GI gI)
    {
        GIRegistry.Register(gI);
    }
}