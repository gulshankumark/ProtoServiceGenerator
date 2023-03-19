using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using ProtoServiceGenerator.Model;

namespace ProtoServiceGenerator.Parser
{
    internal class ProtoParser
    {
        private ProtoParser()
        {
            MessageDefinitions = new Dictionary<string, MessageDefinition>();
            ServiceDefinitions = new Dictionary<string, ServiceDefinition>();
            EnumDefinitions = new Dictionary<string, EnumDefinition>();
        }

        public static ProtoParser Instance { get; } = new ProtoParser();

        public IDictionary<string, EnumDefinition> EnumDefinitions { get; }
        public IDictionary<string, MessageDefinition> MessageDefinitions { get; }
        public IDictionary<string, ServiceDefinition> ServiceDefinitions { get; }

        public void Parse(SourceText content)
        {
            var lines = content.Lines;
            var linesString = lines.Select(x => x.ToString());
            var commentsExcluded = linesString.Where(x => !x.StartsWith("//"));
            var str = string.Join(Environment.NewLine, commentsExcluded);
            HeaderDefinition header = new HeaderDefinition(str);
            var enums = GetBlock(str, "enum");
            var messages = GetBlock(str, "message");
            var services = GetBlock(str, "service");

            foreach (var enumString in enums)
            {
                EnumDefinition enumDefinition = new EnumDefinition(enumString, header);
                EnumDefinitions.Add(enumDefinition.FullyQualifiedProtoName, enumDefinition);
            }

            foreach (var messageString in messages)
            {
                MessageDefinition messageDefinition = new MessageDefinition(messageString, header);
                MessageDefinitions.Add(messageDefinition.FullyQualifiedProtoName, messageDefinition);
            }

            foreach (var serviceString in services)
            {
                ServiceDefinition serviceDefinition = new ServiceDefinition(serviceString, header);
                ServiceDefinitions.Add(serviceDefinition.FullyQualifiedProtoName, serviceDefinition);
            }
        }

        public void GenerateData()
        {

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