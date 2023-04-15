using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoService.Parser.Model
{
    public class MessageDefinition : BaseDefinition, IEqualityComparer<MessageDefinition>
    {
        public MessageDefinition(string messageString, HeaderDefinition header)
        {
            PackageName = header.PackageDefinition.PackageName;
            var tuple = ParseMessageString(messageString);
            Name = tuple.MessageName;
            OptionCSharpNamespace = header.OptionDefinitions.First(x => x.Identifier == "csharp_namespace").Value;
            FullyQualifiedProtoName = $"{PackageName}.{Name}";
            var properties = tuple.Properties;

            MessagePropertyDefinitions = properties.Where(x => !string.IsNullOrEmpty(x)).Select(x => new MessagePropertyDefinition(x)).ToImmutableList();
        }
        
        public IImmutableList<MessagePropertyDefinition> MessagePropertyDefinitions { get; }

        public bool Equals(MessageDefinition x, MessageDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.FullyQualifiedProtoName, y.FullyQualifiedProtoName);
        }

        public int GetHashCode(MessageDefinition obj)
        {
            return (obj.FullyQualifiedProtoName != null ? obj.FullyQualifiedProtoName.GetHashCode() : 0);
        }

        private (string MessageName, IReadOnlyList<string> Properties) ParseMessageString(string messageString)
        {
            messageString = messageString.Replace("\r\n", "").Replace("\t", "");
            var messageName = messageString.Split('{')[0].Trim().Split(' ')[1];
            var messagePropertiesRaw = messageString.Split('{', '}')[1].Trim();
            var messageProperties = messagePropertiesRaw.Split(';');

            return (messageName, messageProperties);
        }
    }
}