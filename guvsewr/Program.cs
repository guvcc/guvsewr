using System.Reflection;
using System.Text.RegularExpressions;


class Program
{
    static void Main(string[] args)
    {
        Console.Title = Config.root.distro_settings.name;

        var allAssemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly()
        };

        allAssemblies.AddRange(CompileScripts.asms);

        var commands = allAssemblies
        .SelectMany(asm => asm.GetTypes())
        .Where(t => t.IsSubclassOf(typeof(Command)) && !t.IsAbstract);

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

            var command = commands.FirstOrDefault(t =>
            {
                var inst = (Command)Activator.CreateInstance(t)!;
                return inst.name.Equals(input.Split(' ')[0], StringComparison.OrdinalIgnoreCase);
            });

            if (command != null)
            {
                var instance = Activator.CreateInstance(command) as Command;
                instance.args = Regex.Matches(input, @"[\""].+?[\""]|\S+") .Select(m => m.Value.Trim('"')).Skip(1).ToArray();
                instance?.executer.Invoke(instance);
            }
            else
            {
                Console.WriteLine(Config.root.cli_settings.errors["command_not_found"]);
            }
        }
    }
}
