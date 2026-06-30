public class CommandAPI
{
    public GuvsewrPackage package { get; set; }

    public void Register(Command command)
    {
        CommandRegistry.Register(command);
    }

    public void Run(string name, string[] args)
    {
        if (CommandRegistry.commands.TryGetValue(name, out Command command))
        {
            command.args = args;
            command?.executer.Invoke(command);
        }
    }
}