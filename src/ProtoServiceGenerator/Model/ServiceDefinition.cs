using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoServiceGenerator.Model
{
    internal class ServiceDefinition : BaseDefinition, IEqualityComparer<ServiceDefinition>
    {
        public ServiceDefinition(string serviceString, HeaderDefinition header)
        {
            PackageName = header.PackageDefinition.PackageName;
            var tuple = ParseServiceString(serviceString);
            Name = tuple.MessageName;
            FullyQualifiedProtoName = $"{PackageName}.{Name}";
            var properties = tuple.Properties;

            RpcDefinitions = properties.Where(x => !string.IsNullOrEmpty(x)).Select(x => new RpcDefinition(x, header)).ToImmutableList();
        }
        
        public IImmutableList<RpcDefinition> RpcDefinitions { get; }

        public bool Equals(ServiceDefinition x, ServiceDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.FullyQualifiedProtoName, y.FullyQualifiedProtoName);
        }

        public int GetHashCode(ServiceDefinition obj)
        {
            return (obj.FullyQualifiedProtoName != null ? obj.FullyQualifiedProtoName.GetHashCode() : 0);
        }

        private (string MessageName, IReadOnlyList<string> Properties) ParseServiceString(string serviceString)
        {
            serviceString = serviceString.Replace("\r\n", "").Replace("\t", "");
            var serviceName = serviceString.Split(' ')[1];
            var rpcRaw = serviceString.Split('{', '}')[1];
            var rpcs = rpcRaw.Split(';');

            return (serviceName, rpcs);
        }
    }
}