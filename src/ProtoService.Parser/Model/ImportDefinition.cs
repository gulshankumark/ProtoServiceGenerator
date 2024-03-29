﻿using System.Linq;

namespace Proto.Service.Parser.Model
{
    public class ImportDefinition
    {
        public ImportDefinition(string importProtoName)
        {
            ImportProtoName = GetImportName(importProtoName);
        }

        public string ImportProtoName { get; }

        private string GetImportName(string importName)
        {
            importName = importName.Replace("\r\n", " ");
            var lastCharacterRemoved = importName.Replace(";", "");
            return lastCharacterRemoved.Split(' ').Last().Replace("\"", "");
        }
    }
}