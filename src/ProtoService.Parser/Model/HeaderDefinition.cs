using System;
using System.Collections.Generic;
using System.Linq;

namespace Proto.Service.Parser.Model
{
    public class HeaderDefinition
    {
        public HeaderDefinition(string headerString)
        {
            var importContents = GetContent(headerString, "import", ';');
            var optionContents = GetContent(headerString, "option", ';');
            var packageContent = GetPackageContent(headerString);

            PackageDefinition = new PackageDefinition(packageContent);
            ImportDefinitions = importContents.Select(x => new ImportDefinition(x)).ToList();
            OptionDefinitions = optionContents.Select(x => new OptionDefinition(x)).ToList();
        }

        public PackageDefinition PackageDefinition { get; }
        public IReadOnlyList<ImportDefinition> ImportDefinitions { get; }
        public IReadOnlyList<OptionDefinition> OptionDefinitions { get; }

        private IReadOnlyList<string> GetContent(string str, string identifier, char limiter)
        {
            var list = new List<string>();
            var startIndex = 0;
            while (true)
            {
                var index = str.IndexOf(identifier, startIndex, StringComparison.Ordinal);
                if (index == -1)
                {
                    break;
                }

                var endIndex = str.IndexOf(limiter, index);
                startIndex = endIndex;
                list.Add(str.Substring(index, endIndex - index + 1));
            }

            return list;
        }
        private string GetPackageContent(string str)
        {
            var index = str.IndexOf("package", StringComparison.Ordinal);
            if (index == -1)
            {
                return null;
            }

            var endIndex = str.IndexOf(';', index);
            return str.Substring(index, endIndex - index + 1);
        }
    }
}