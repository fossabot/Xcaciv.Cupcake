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
    [CommandParameterOrdered("search string", "String associated to the desired package.", IsRequired = true)]
    public class SearchCommand : AbstractCommand
    {
        public override string HandleExecution(string[] parameters, IEnvironmentContext env)
        {
            // return "Not searching " + String.Join(',', parameters);
            var packageSourceUrl = env.GetValue("PackageSourceUrl");
            return String.Join("\n", this.SearchNugetFeed(String.Join(',', parameters), packageSourceUrl));
        }

        public override string HandlePipedChunk(string pipedChunk, string[] parameters, IEnvironmentContext env)
        {
            return $"Unsupported search method for {pipedChunk} " + String.Join(',', parameters);
        }

        protected async Task<List<string>> SearchNugetFeed(string searchTerm, string packageSourceUrl)
        {
            List<string> packageNames = new List<string>();

            // "https://api.nuget.org/v3/index.json"
            var nugetSource = new PackageSource(packageSourceUrl);
            SourceRepository repository = Repository.Factory.GetCoreV3(nugetSource);

            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();

            using (var sourceCacheContext = new SourceCacheContext())
            {
                SearchFilter searchFilter = new SearchFilter(includePrerelease: false);
                IEnumerable<IPackageSearchMetadata> packages = await resource.SearchAsync(searchTerm, searchFilter, skip: 0, take: 100, NullLogger.Instance, CancellationToken.None);

                foreach (var package in packages)
                {
                    packageNames.Add(package.Identity.Id);
                }
            }

            return packageNames;
        }
    }
}
