using System.Collections.Generic;

namespace ProtoServiceGenerator.Model
{
    internal class MessagePropertyDefinition
    {
        private static readonly IDictionary<string, MessagePropertyType> map =
            new Dictionary<string, MessagePropertyType>
            {
                { "string", MessagePropertyType.StringVal },
                { "int32", MessagePropertyType.Int32Val },
                { "int64", MessagePropertyType.Int64Val },
                { "double", MessagePropertyType.DoubleVal },
                { "float", MessagePropertyType.FloatVal },
                { "uint32", MessagePropertyType.UInt32Val },
                { "uint64", MessagePropertyType.UInt64Val },
                { "sint32", MessagePropertyType.SInt32Val },
                { "sint64", MessagePropertyType.SInt64Val },
                { "fixed32", MessagePropertyType.Fixed32Val },
                { "fixed64", MessagePropertyType.Fixed64Val },
                { "sfixed32", MessagePropertyType.SFixed32Val },
                { "sfixed64", MessagePropertyType.SFixed64Val },
                { "bool", MessagePropertyType.BoolVal },
                { "bytes", MessagePropertyType.BytesVal }
            };

        public MessagePropertyDefinition(string messagePropertyString)
        {
            messagePropertyString = messagePropertyString.Trim();

            if (messagePropertyString.StartsWith("repeated"))
            {
                IsRepeated = true;
                messagePropertyString = messagePropertyString.Replace("repeated", "").Trim();
            }

            var splits = messagePropertyString.Replace(";", "").Split(' ');
            var type = splits[0];
            Name = splits[1];
            Index = int.Parse(splits[splits.Length - 1]);

            if (map.ContainsKey(type))
            {
                MessagePropertyType = map[type];
            }
            else
            {
                MessagePropertyType = MessagePropertyType.OtherMessageVal;
                OtherMessageDefinition = type;
            }

        }

        public int Index { get; }
        public MessagePropertyType MessagePropertyType { get; }
        public string OtherMessageDefinition { get; }
        public string Name { get; }
        public bool IsRepeated { get; }
    }

    // Reference: https://developers.google.com/protocol-buffers/docs/proto3#scalar
    internal enum MessagePropertyType
    {
        DoubleVal,
        FloatVal,
        Int32Val,
        Int64Val,
        UInt32Val,
        UInt64Val,
        SInt32Val,
        SInt64Val,
        Fixed32Val,
        Fixed64Val,
        SFixed32Val,
        SFixed64Val,
        BoolVal,
        StringVal,
        BytesVal,
        EnumVal,
        ListVal,
        MapVal,
        AnyVal,
        EmptyVal,
        OtherMessageVal
    }
}