using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Proto.Service.Parser.Model;
using Proto.Service.Parser.Parser;

namespace Proto.Service.Interface.Generator
{
    [Generator]
    public class InterfaceGenerator : ISourceGenerator
    {
        private readonly ParserMap _parserMap;
        private readonly ProtoParser _parser;

        public InterfaceGenerator()
        {
            _parserMap = new ParserMap();
            _parser = new ProtoParser(_parserMap);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var protoFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".proto", StringComparison.OrdinalIgnoreCase)
                                                                 && !at.Path.EndsWith("Client.proto", StringComparison.OrdinalIgnoreCase));
            foreach (var file in protoFiles)
            {
                var content = file.GetText(context.CancellationToken);
                ReadProtoFile(content);
            }

            GenerateData(context);
        }

        private void ReadProtoFile(SourceText content)
        {
            _parser.Parse(content);
        }

        private void GenerateData(GeneratorExecutionContext context)
        {
            foreach (var serviceDefinition in _parserMap.ServiceDefinitions)
            {
                var code = EmitCode(serviceDefinition.Value);
                context.AddSource($"{serviceDefinition.Key}.g.cs", code);
            }
        }

        private string EmitCode(ServiceDefinition serviceDefinition)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"// This is auto-generated code from {nameof(InterfaceGenerator)}");
            builder.AppendLine($"namespace {serviceDefinition.OptionCSharpNamespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic interface I{serviceDefinition.Name}");
            builder.AppendLine("\t{");
            builder.AppendLine("#nullable enable");
            foreach (var rpcDefinition in serviceDefinition.RpcDefinitions)
            {
                builder.AppendLine($"\t\t{rpcDefinition.ResponseParameter.ToResponseParameter()} {rpcDefinition.RpcName}({rpcDefinition.InParameter.ToServiceInputParameter(true)});");
            }

            builder.AppendLine("#nullable disable");
            builder.AppendLine("\t}");
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
