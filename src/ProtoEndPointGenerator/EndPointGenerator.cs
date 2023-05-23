using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Proto.Service.Parser.Model;
using Proto.Service.Parser.Parser;

namespace Proto.Service.ProtoEndPoint.Generator
{
    [Generator]
    public class EndPointGenerator : ISourceGenerator
    {
        private readonly ParserMap _parserMap;
        private readonly ProtoParser _parser;
        public EndPointGenerator()
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
            _parser.SetAssemblyName(context.Compilation.AssemblyName);
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
                SourceText text = SourceText.From(code, Encoding.UTF8);
                context.AddSource($"{serviceDefinition.Key}.g.cs", text);
            }
        }

        private string EmitCode(ServiceDefinition serviceDefinition)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"// This is auto-generated code from {nameof(EndPointGenerator)}");
            builder.AppendLine($"namespace {_parserMap.AssemblyName}.Services;");
            builder.AppendLine($"[System.CodeDom.Compiler.GeneratedCode(\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]");
            var serviceEndpointName = $"{serviceDefinition.Name}EndPoint";
            var serviceTypeName = $"{serviceDefinition.OptionCSharpNamespace}.I{serviceDefinition.Name}";
            var loggerTypeName = $"Microsoft.Extensions.Logging.ILogger<{serviceEndpointName}>";
            builder.AppendLine($"public partial class {serviceEndpointName} : {serviceDefinition.OptionCSharpNamespace}.{serviceDefinition.Name}.{serviceDefinition.Name}Base");
            builder.AppendLine("{");
            builder.AppendLine($"\tprivate readonly {serviceTypeName} _service;");
            builder.AppendLine($"\tprivate readonly {loggerTypeName} _logger;");

            builder.AppendLine($"\tpublic {serviceEndpointName}({serviceTypeName} service, {loggerTypeName} logger)");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\t_service = service;");
            builder.AppendLine("\t\t_logger = logger;");
            builder.AppendLine("\t}");

            foreach (var rpcDefinition in serviceDefinition.RpcDefinitions)
            {
                var requestToServiceName = string.IsNullOrEmpty(rpcDefinition.InParameter.Name) ? "context" : "request, context";
                var inputParam = rpcDefinition.InParameter.ToInputParameter(false);
                //var requestToServiceName = inputParam.Contains("request") ? "request, context" : "context";
                builder.AppendLine(
                    $"\tpublic override async partial {rpcDefinition.ResponseParameter.ToResponseParameter()} {rpcDefinition.RpcName}({inputParam})");
                builder.AppendLine("\t{");
                builder.AppendLine($"\t\treturn await _service.{rpcDefinition.RpcName}({requestToServiceName});");
                builder.AppendLine("\t}");
                //builder.AppendLine($"\t{rpcDefinition.ResponseParameter.ToResponseParameter()} {rpcDefinition.RpcName}({rpcDefinition.InParameter.ToInputParameter()});");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
