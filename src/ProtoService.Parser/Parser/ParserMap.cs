using System.Collections.Generic;
using ProtoService.Parser.Model;

namespace ProtoService.Parser.Parser
{
    public class ParserMap
    {
        public ParserMap()
        {
            MessageDefinitions = new Dictionary<string, MessageDefinition>();
            ServiceDefinitions = new Dictionary<string, ServiceDefinition>();
            EnumDefinitions = new Dictionary<string, EnumDefinition>();
        }

        public string AssemblyName { get; private set; }
        public IDictionary<string, EnumDefinition> EnumDefinitions { get; }
        public IDictionary<string, MessageDefinition> MessageDefinitions { get; }
        public IDictionary<string, ServiceDefinition> ServiceDefinitions { get; }

        public void SetAssemblyName(string assemblyName)
        {
            AssemblyName = assemblyName;
        }
    }
}