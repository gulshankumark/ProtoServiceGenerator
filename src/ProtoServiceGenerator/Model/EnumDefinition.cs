using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoServiceGenerator.Model
{
    internal class EnumDefinition : BaseDefinition, IEqualityComparer<BaseDefinition>
    {
        public EnumDefinition(string messageString, HeaderDefinition header)
        {
            PackageName = header.PackageDefinition.PackageName;
            var tuple = ParseMessageString(messageString);
            Name = tuple.EnumName;
            FullyQualifiedProtoName = $"{PackageName}.{Name}";
            var values = tuple.Values;

            EnumValueDefinitions = values.Where(x => !string.IsNullOrEmpty(x)).Select(x => new EnumValueDefinition(x)).ToImmutableList();
        }

        private (string EnumName, IReadOnlyList<string> Values) ParseMessageString(string enumString)
        {
            enumString = enumString.Replace("\r\n", "").Replace("\t", "");
            var enumName = enumString.Split(' ')[1];
            var enumValuesRaw = enumString.Split('{', '}')[1];
            var enumValue = enumValuesRaw.Split(';');

            return (enumName, enumValue);
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
    }
}