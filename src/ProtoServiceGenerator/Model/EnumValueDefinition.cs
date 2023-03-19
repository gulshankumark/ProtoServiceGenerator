namespace ProtoServiceGenerator.Model
{
    internal class EnumValueDefinition
    {
        public EnumValueDefinition(string enumValueString)
        {
            enumValueString = enumValueString.Trim();
            
            var splits = enumValueString.Replace(";", "").Split(' ');
            Name = splits[0];
            Index = int.Parse(splits[splits.Length - 1]);
        }

        public int Index { get; }
        public string Name { get; }
    }
}