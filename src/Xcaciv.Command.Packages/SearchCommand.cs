using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Xcaciv.Command.Core;
using Xcaciv.Command.Interface;
using Xcaciv.Command.Interface.Attributes;

namespace Xcaciv.Command.Packages
{
    [CommandRoot("Package", "Package commands")]
    [CommandRegister("Search", "install a package")]
    [CommandParameterOrdered("search_terms", "String associated to the desired package.", IsRequired = true)]
    [CommandParameterNamed("source", "The source to search for the package.")]
    [CommandParameterNamed("take", "Limit the number of results to return.", DefaultValue = "20")]
    [CommandParameterNamed("verbosity", "The level of detail to display in the output.", AllowedValues = ["quiet", "normal", "detailed"], DefaultValue = "normal")]
    [CommandFlag("prerelease", "Include prerelease packages in the search results.")]
    public class SearchCommand : AbstractCommand
    {
        public override string HandleExecution(string[] parameters, IEnvironmentContext env)
        {
            var parameterDictionary = this.ProcessParameters(parameters);

            var packageSourceUrl = env.GetValue("PackageSourceUrl");
            // default to nuget.org
            if (String.IsNullOrEmpty(packageSourceUrl))
            {
                packageSourceUrl = "https://api.nuget.org/v3/index.json";
            }

            List<string> searchResult = [];
            int limit = int.Parse(parameterDictionary["take"]);
            bool prerelease = parameterDictionary.ContainsKey("prerelease");
            
            var tmpResult = NugetWrapper.FindPackageAsync(parameterDictionary["search_terms"], packageSourceUrl, limit, prerelease).Result;

            switch (parameterDictionary["verbosity"])
            {
                case "quiet":
                    searchResult = tmpResult.Select(p => p.Identity.Id).ToList();
                    break;
                case "normal":
                    searchResult = tmpResult.Select(p => $"{p.Identity.Id} {p.Identity.Version} : {p.Summary}").ToList();
                    break;
                case "detailed":
                    searchResult = tmpResult.Select(p => $"{p.Identity.Id} {p.Identity.Version} ({p.DownloadCount}) : {p.Summary}" +
                    $"\n  Published:{p.Published}" +
                    $"\n  Authors:{p.Authors}" +
                    $"\n  License:{p.LicenseMetadata}" +
                    $"\n  Vulnerabilities:{p.Vulnerabilities.Count()}" +
                    $"\n---").ToList();
                    break;
            }

            return String.Join("\n", searchResult);
        }

        public override string HandlePipedChunk(string pipedChunk, string[] parameters, IEnvironmentContext env)
        {
            return $"Unsupported search method for {pipedChunk} (piped)" + String.Join(',', parameters);
        }

    }
}
