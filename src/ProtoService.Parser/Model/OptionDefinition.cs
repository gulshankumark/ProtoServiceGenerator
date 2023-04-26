using System.Linq;

namespace Proto.Service.Parser.Model
{
    public class OptionDefinition
    {
        public OptionDefinition(string optionString)
        {
            var tuple = ParseOptionString(optionString);
            Identifier = tuple.Identifier;
            Value = tuple.Value;
        }

        public string Identifier { get; }
        public string Value { get; }

        private (string Identifier, string Value) ParseOptionString(string optionString)
        {
            optionString = optionString.Replace("\r\n", " ");
            var lastCharacterRemoved = optionString.Replace(";", "");
            var splits = lastCharacterRemoved.Split(' ');
            var identifier = splits[1];
            var value = splits.Last().Replace("\"", "");
            return (identifier, value);
        }
    }
}