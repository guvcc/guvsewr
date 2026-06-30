public class Command
{
    public string name;
    public Action<Command> executer;
    public string[] args;

    public Command(string name, Action<Command> executer, string[] args = null)
    {
        this.name = name;
        this.args = args;
        this.executer = executer;
    }
}
public static class CommandRegistry
{
    public static readonly Dictionary<string, Command> commands = new();

    public static void Register(Command command)
    {
        commands[command.name] = command;
    }
}
