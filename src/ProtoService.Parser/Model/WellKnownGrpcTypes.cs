using System.Collections.Generic;

namespace Proto.Service.Parser.Model
{
    public static class WellKnownGrpcTypes
    {
        static WellKnownGrpcTypes()
        {
            Types = new Dictionary<string, WellKnownTypeDefinition>
            {
                { "google.protobuf.Empty", new WellKnownTypeDefinition(string.Empty, "google/protobuf/empty.proto", "google.protobuf.Empty") }
            };
        }

        public static Dictionary<string, WellKnownTypeDefinition> Types;
    }
}