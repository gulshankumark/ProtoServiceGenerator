using System.Linq;
using ProtoServiceGenerator.Parser;

namespace ProtoServiceGenerator.Model
{
    internal class RpcDefinition
    {
        public RpcDefinition(string rpcString, HeaderDefinition headerDefinition)
        {
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
            if (ProtoParser.Instance.EnumDefinitions.ContainsKey(fullyQualifiedName))
            {
                return ProtoParser.Instance.EnumDefinitions[fullyQualifiedName];
            }

            foreach (var importDefinition in headerDefinition.ImportDefinitions)
            {
                fullyQualifiedName = $"{importDefinition.ImportProtoName}";
            }

            return null;
        }
    }
}