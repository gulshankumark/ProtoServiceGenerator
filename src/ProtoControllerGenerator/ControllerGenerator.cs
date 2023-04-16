using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ProtoService.Parser.Model;
using ProtoService.Parser.Parser;

namespace ProtoControllerGenerator
{
    [Generator]
    public class ControllerGenerator : ISourceGenerator
    {
        private readonly ParserMap _parserMap;
        private readonly ProtoParser _parser;
        public ControllerGenerator()
        {
            _parserMap = new ParserMap();
            _parser = new ProtoParser(_parserMap);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
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
            builder.AppendLine($"namespace {_parserMap.AssemblyName}.Controllers;");
            var serviceControllerName = $"{serviceDefinition.Name}Controller";
            var serviceTypeName = $"{serviceDefinition.OptionCSharpNamespace}.I{serviceDefinition.Name}";
            var loggerTypeName = $"Microsoft.Extensions.Logging.ILogger<{serviceControllerName}>";
            builder.AppendLine("[Microsoft.AspNetCore.Mvc.ApiController]");
            builder.AppendLine($"public partial class {serviceControllerName} : Microsoft.AspNetCore.Mvc.ControllerBase");
            builder.AppendLine("{");
            builder.AppendLine($"\tprivate readonly {serviceTypeName} _service;");
            builder.AppendLine($"\tprivate readonly {loggerTypeName} _logger;");

            builder.AppendLine($"\tpublic {serviceControllerName}({serviceTypeName} service, {loggerTypeName} logger)");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\t_service = service;");
            builder.AppendLine("\t\t_logger = logger;");
            builder.AppendLine("\t}");

            foreach (var rpcDefinition in serviceDefinition.RpcDefinitions)
            {
                var inputParam = rpcDefinition.InParameter.ToControllerInputParameter();
                var requestToServiceName = string.IsNullOrEmpty(rpcDefinition.InParameter.Name) ? string.Empty : "request";
                builder.AppendLine(inputParam != null
                    ? $"\t[Microsoft.AspNetCore.Mvc.HttpPost(nameof({rpcDefinition.RpcName}))]"
                    : $"\t[Microsoft.AspNetCore.Mvc.HttpGet(nameof({rpcDefinition.RpcName}))]");
                builder.AppendLine($"\tpublic async partial Task<Microsoft.AspNetCore.Mvc.IActionResult> {rpcDefinition.RpcName}({inputParam})");
                builder.AppendLine("\t{");
                builder.AppendLine("\t\ttry");
                builder.AppendLine("\t\t{");
                builder.AppendLine($"\t\t\tvar response = await _service.{rpcDefinition.RpcName}({requestToServiceName});");
                builder.AppendLine("\t\t\treturn Ok(response);");
                builder.AppendLine("\t\t}");
                builder.AppendLine("\t\tcatch (Exception ex)");
                builder.AppendLine("\t\t{");
                builder.AppendLine("\t\t\treturn BadRequest();");
                builder.AppendLine("\t\t}");
                builder.AppendLine("\t}");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
