public class GIExecutor
{
    public Action<Command> executer;
    public GI gI { get; set; }

    public string name;

    public GIExecutor(string name)
    {
        this.name = name;

        executer = (cmd) =>
        {
            if (cmd.args.Length == 0)
            {
                Console.WriteLine(Config.root.cli_settings.errors["invalid_arguments"]);
                return;
            }

            if (cmd.args[0] == "start")
            {
                gI.Start?.Invoke(gI);
            }

            if (cmd.args[0] == "stop")
            {
                gI.Stop?.Invoke(gI);
            }
        };
    }
}