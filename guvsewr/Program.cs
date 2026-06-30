using System.Reflection;
using System.Text.RegularExpressions;


class Program
{
    static void Main(string[] args)
    {
        Console.Title = Config.root.distro_settings.name;

        Commands.Register();

        CompileScripts.CompileAll("packages");

        foreach (string sentence in Config.root.introduction_sentences)
        {
            Console.WriteLine(sentence);
        }

        while (true)
        {
            Console.Write(Config.root.cli_settings.line_starter + " ");

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(Config.root.cli_settings.errors["empty_command"]);
                continue;
            }

            var cmd = input.Split(' ')[0];

            if (CommandRegistry.commands.TryGetValue(cmd, out Command command))
            {
                command.args = Regex.Matches(input, @"[\""].+?[\""]|\S+").Select(m => m.Value.Trim('"')).Skip(1).ToArray();
                command?.executer.Invoke(command);
            }
            else
            {
                Console.WriteLine(Config.root.cli_settings.errors["command_not_found"]);
            }
        }
    }
}
