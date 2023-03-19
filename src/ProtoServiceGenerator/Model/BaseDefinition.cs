using System.Collections.Generic;

namespace ProtoServiceGenerator.Model
{
    public class BaseDefinition
    {
        public string FullyQualifiedProtoName { get; protected set; }
        public string PackageName { get; protected set; }
        public string Name { get; protected set; }
    }
}