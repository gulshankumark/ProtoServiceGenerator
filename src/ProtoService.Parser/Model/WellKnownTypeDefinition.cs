using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace ProtoService.Parser.Model
{
    public class WellKnownTypeDefinition : BaseDefinition
    {
        private static readonly IDictionary<string, string> ProtoCSharpMapping;

        static WellKnownTypeDefinition()
        {
            ProtoCSharpMapping = new Dictionary<string, string>
            {
                { "int32", "Int32" },
                { "int64", "Int64" },
                { "int16", "Int16" },
                { "uint32", "UInt32" },
                { "uint64", "UInt64" },
                { "uint16", "UInt16" }
            };
        }

        public WellKnownTypeDefinition(string name, string packageName, string fullyQualifiedName)
        {
            OptionCSharpNamespace = string.Empty;
            Name = name;
            PackageName = packageName;
            FullyQualifiedProtoName = fullyQualifiedName;
        }

        public override string ToInputParameter(bool isNullableContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return $"Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.ServerCallContext{(isNullableContext ? "?" : string.Empty)} context{(isNullableContext ? " = null" : string.Empty)}";
            }

            return $"{ProtoCSharpMapping[Name]} request, Grpc.Core.ServerCallContext{(isNullableContext ? "?" : string.Empty)} context{(isNullableContext ? " = null" : string.Empty)}";
        }

        public override string ToControllerInputParameter()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return string.Empty;
            }

            return $"{ProtoCSharpMapping[Name]} request";
        }

        public override string ToServiceInputParameter(bool isNullableContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return $"Grpc.Core.ServerCallContext{(isNullableContext ? "?" : string.Empty)} context{(isNullableContext ? " = null" : string.Empty)}";
            }

            return $"{ProtoCSharpMapping[Name]} request, Grpc.Core.ServerCallContext{(isNullableContext ? "?" : string.Empty)} context{(isNullableContext ? " = null" : string.Empty)}";
        }

        public override string ToResponseParameter()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "Task";
            }

            return $"Task<{ProtoCSharpMapping[Name]}";
        }
    }
}