namespace Proto.Service.Parser.Model
{
    public class BaseDefinition
    {
        public string OptionCSharpNamespace { get; protected set; }
        public string FullyQualifiedProtoName { get; protected set; }
        public string PackageName { get; protected set; }
        public string Name { get; protected set; }

        public virtual string ToInputParameter(bool isNullableContext)
        {
            return $"{OptionCSharpNamespace}.{Name} request, Grpc.Core.ServerCallContext{(isNullableContext ? "?" : string.Empty)} context{(isNullableContext ? " = null" : string.Empty)}";
        } 
        
        public virtual string ToControllerInputParameter()
        {
            return $"{OptionCSharpNamespace}.{Name} request";
        }

        public virtual string ToServiceInputParameter(bool isNullableContext)
        {
            return ToInputParameter(isNullableContext);
        }

        public virtual string ToResponseParameter()
        {
            return $"Task<{OptionCSharpNamespace}.{Name}>";
        }
    }
}