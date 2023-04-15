using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ProtoServiceGenerator.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Single_File_Is_Added()
        {
            var source = @"
class C { }
";

            var generatorSource = @"
class GeneratedClass { }
";

            var parseOptions = TestOptions.Regular;
            Compilation compilation = CreateCompilation(source, options: TestOptions.DebugDllThrowing, parseOptions: parseOptions);
            compilation.VerifyDiagnostics();

            Assert.Single(compilation.SyntaxTrees);

            InterfaceGenerator testGenerator = new InterfaceGenerator(generatorSource);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { testGenerator }, parseOptions: parseOptions);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

            Assert.Equal(2, outputCompilation.SyntaxTrees.Count());
            Assert.NotEqual(compilation, outputCompilation);

            var generatedClass = outputCompilation.GlobalNamespace.GetTypeMembers("GeneratedClass").Single();
            Assert.True(generatedClass.Locations.Single().IsInSource);
        }

    }
}