using System.Linq;
using ProtoService.Parser.Parser;

namespace ProtoService.Parser.Model
{
    public class RpcDefinition
    {
        private readonly ParserMap _parserMap;

        public RpcDefinition(ParserMap parserMap, string rpcString, HeaderDefinition headerDefinition)
        {
            _parserMap = parserMap;
            ParseRpcString(rpcString, headerDefinition);
        }

        private void ParseRpcString(string rpcString, HeaderDefinition headerDefinition)
        {
            var splits = rpcString.Split(' ');
            RpcName = splits[1];
            var splitsParam = rpcString.Split('(', ')').Where(x => !string.IsNullOrEmpty(x)).ToList();
            var inParamString = splitsParam[1];
            if (inParamString.StartsWith("stream"))
            {
                IsRequestStream = true;
                inParamString = inParamString.Replace("stream", "").Trim();
            }

            var outParamString = splitsParam[3];
            if (outParamString.StartsWith("stream"))
            {
                IsResponseStream = true;
                outParamString = outParamString.Replace("stream", "").Trim();
            }

            InParameter = GetParameter(inParamString, headerDefinition);
            ResponseParameter = GetParameter(outParamString, headerDefinition);
        }

        public bool IsRequestStream { get; private set; }
        public bool IsResponseStream { get; private set; }
        public string RpcName { get; private set; }
        public BaseDefinition InParameter { get; private set; }
        public BaseDefinition ResponseParameter { get; private set; }

        private BaseDefinition GetParameter(string paramString, HeaderDefinition headerDefinition)
        {
            var fullyQualifiedName = $"{headerDefinition.PackageDefinition.PackageName}.{paramString}";
            if (_parserMap.EnumDefinitions.ContainsKey(fullyQualifiedName))
            {
                return _parserMap.EnumDefinitions[fullyQualifiedName];
            }

            if (_parserMap.MessageDefinitions.ContainsKey(fullyQualifiedName))
            {
                return _parserMap.MessageDefinitions[fullyQualifiedName];
            }

            return WellKnownGrpcTypes.Types[paramString];
        }
    }
}