using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

public class CompileScripts
{
    public static List<Assembly> asms;
    public static Assembly Compile(string path)
    {
        var code = File.ReadAllText(path);

        var tree = CSharpSyntaxTree.ParseText(code);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        var compilation = CSharpCompilation.Create(
            Path.GetRandomFileName(),
            new[] { tree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error);

            foreach (var e in errors)
                Console.WriteLine(e);

            throw new Exception(Config.root.cli_settings.errors["no_load_script"]);
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    public void CompileScript(string path)
    {
        foreach (string directory in Directory.GetDirectories(path))
        {
            CompileScript(directory);
        }

        foreach (string cs in Directory.GetFiles(path))
        {
            if (cs.EndsWith(".cs"))
            {
                var asm = Compile(cs);
                asms.Add(asm);
            }
        }
    }
}