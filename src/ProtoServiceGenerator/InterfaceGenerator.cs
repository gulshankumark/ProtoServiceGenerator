using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using ProtoServiceGenerator.Parser;

namespace ProtoServiceGenerator
{
    [Generator]
    public class InterfaceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif

            context.RegisterForSyntaxNotifications(() => new ProtoSyntaxReceiver());
        }
        
        public void Execute(GeneratorExecutionContext context)
        {
            var protoFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".proto"));
            foreach (var file in protoFiles)
            {
                var content = file.GetText(context.CancellationToken);

                // do some transforms based on the file context
                ReadProtoFile(content);

                //context.AddSource($"{file.Path}generated.cs", sourceText);
            }

            GenerateData();
            //throw new System.NotImplementedException();
        }

        private void GenerateData()
        {
            ProtoParser.Instance.GenerateData();
        }

        private void ReadProtoFile(SourceText content)
        {
            ProtoParser.Instance.Parse(content);
        }
    }
}
