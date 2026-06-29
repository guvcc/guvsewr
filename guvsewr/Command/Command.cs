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
