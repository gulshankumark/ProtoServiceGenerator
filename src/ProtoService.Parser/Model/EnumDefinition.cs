using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Proto.Service.Parser.Model
{
    public class EnumDefinition : BaseDefinition, IEqualityComparer<BaseDefinition>
    {
        public EnumDefinition(string messageString, HeaderDefinition header)
        {
            PackageName = header.PackageDefinition.PackageName;
            var tuple = ParseMessageString(messageString);
            Name = tuple.EnumName;
            OptionCSharpNamespace = header.OptionDefinitions.First(x => x.Identifier == "csharp_namespace").Value;
            FullyQualifiedProtoName = $"{PackageName}.{Name}";
            var values = tuple.Values;

            EnumValueDefinitions = values.Where(x => !string.IsNullOrEmpty(x)).Select(x => new EnumValueDefinition(x)).ToImmutableList();
        }
        public IImmutableList<EnumValueDefinition> EnumValueDefinitions { get; }
        
        public bool Equals(BaseDefinition x, BaseDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.FullyQualifiedProtoName == y.FullyQualifiedProtoName;
        }

        public int GetHashCode(BaseDefinition obj)
        {
            return (obj.FullyQualifiedProtoName != null ? obj.FullyQualifiedProtoName.GetHashCode() : 0);
        }

        private (string EnumName, IReadOnlyList<string> Values) ParseMessageString(string enumString)
        {
            enumString = enumString.Replace("\r\n", "").Replace("\t", "");
            var enumName = enumString.Split('{')[0].Trim().Split(' ')[1];
            var enumValuesRaw = enumString.Split('{', '}')[1].Trim();
            var enumValue = enumValuesRaw.Split(';');

            return (enumName, enumValue);
        }
    }
}