public class Clear : Command
{
    public Clear() : base("clear", (cmd) =>
    {
        Console.Clear();
    })
    { }
}
