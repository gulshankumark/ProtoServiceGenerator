using System.Linq;

namespace ProtoServiceGenerator.Model
{
    internal class PackageDefinition
    {
        public PackageDefinition(string packageString)
        {
            PackageName = GetPackageName(packageString);
        }

        public string PackageName { get; }

        private string GetPackageName(string packageString)
        {
            packageString = packageString.Replace("\r\n", " ");
            var lastCharacterRemoved = packageString.Replace(";", "");
            return lastCharacterRemoved.Split(' ').Last();
        }
    }
}