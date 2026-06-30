using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

public class CompileScripts
{
    public static Assembly Compile(string path, Package gpack)
    {
        var code = File.ReadAllText(path);

        var trees = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file)));

        var references = new List<MetadataReference>();

        references.AddRange(
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location)));

        string nugetFolder = Path.Combine(path, ".nuget");

        foreach (string dll in Directory.GetFiles(
        nugetFolder,
        "*.dll",
        SearchOption.AllDirectories))
        {
            references.Add(MetadataReference.CreateFromFile(dll));
        }

        var compilation = CSharpCompilation.Create(
            Path.GetRandomFileName(),
            trees,
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
        var asm = Assembly.Load(ms.ToArray());

        var type = asm.GetTypes().FirstOrDefault(t => !t.IsAbstract && t.IsSubclassOf(typeof(GuvsewrPackage)));
        var instance = (GuvsewrPackage)Activator.CreateInstance(type, gpack, path, asm);

        GuvsewrPackage.Packages.Add(instance);

        return asm;
    }

    public static void CompileAll(string path)
    {
        if (!Directory.Exists(path))
            return;

        foreach (string dir in Directory.GetDirectories(path))
        {
            var pack = Package.DeserealizeGPack(File.ReadAllText(Path.Combine(dir, "pack.gpack")));

            CompileScripts.Compile(dir, pack);
        }
    }
}