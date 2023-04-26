using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Proto.Service.Parser.Model;

namespace Proto.Service.Parser.Parser
{
    public class ProtoParser
    {
        private readonly ParserMap _parserMap;

        public ProtoParser(ParserMap parserMap)
        {
            _parserMap = parserMap;
        }

        public void SetAssemblyName(string assemblyName)
        {
            _parserMap.SetAssemblyName(assemblyName);
        }

        public void Parse(SourceText content)
        {
            var lines = content.Lines;
            var linesString = lines.Select(x => x.ToString());
            var commentsExcluded = linesString.Where(x => !x.Trim().StartsWith("//"));
            var str = string.Join(Environment.NewLine, commentsExcluded);
            HeaderDefinition header = new HeaderDefinition(str);
            var enums = GetBlock(str, "enum");
            var messages = GetBlock(str, "message");
            var services = GetBlock(str, "service");

            foreach (var enumString in enums)
            {
                EnumDefinition enumDefinition = new EnumDefinition(enumString, header);
                _parserMap.EnumDefinitions.Add(enumDefinition.FullyQualifiedProtoName, enumDefinition);
            }

            foreach (var messageString in messages)
            {
                MessageDefinition messageDefinition = new MessageDefinition(messageString, header);
                _parserMap.MessageDefinitions.Add(messageDefinition.FullyQualifiedProtoName, messageDefinition);
            }

            foreach (var serviceString in services)
            {
                ServiceDefinition serviceDefinition = new ServiceDefinition(_parserMap, serviceString, header);
                _parserMap.ServiceDefinitions.Add(serviceDefinition.FullyQualifiedProtoName, serviceDefinition);
            }
        }

        private IReadOnlyList<string> GetBlock(string str, string identifier)
        {
            var list = new List<string>();
            var startIndex = 0;
            while (true)
            {
                var index = str.IndexOf(identifier, startIndex, StringComparison.Ordinal);
                if (index == -1)
                {
                    break;
                }

                var endIndex = str.IndexOf('}', index);
                startIndex = endIndex;
                list.Add(str.Substring(index, endIndex - index + 1));
            }

            return list;
        }
    }
}