using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Packaging;
using NuGet.Versioning;

namespace Xcaciv.Command.Packages
{
    internal class NugetWrapper
    {
        public static async Task<List<IPackageSearchMetadata>> FindPackageAsync(string searchTerm, string packageSourceUrl, int limit = 20, bool prerelease = false)
        {
            List<string> packageNames = new List<string>();

            var nugetSource = new PackageSource(packageSourceUrl);
            SourceRepository repository = Repository.Factory.GetCoreV3(nugetSource);

            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();

            using (var sourceCacheContext = new SourceCacheContext())
            {
                SearchFilter searchFilter = new SearchFilter(includePrerelease: prerelease);
                IEnumerable<IPackageSearchMetadata> packages = await resource.SearchAsync(searchTerm, searchFilter, skip: 0, take: limit, NullLogger.Instance, CancellationToken.None);

                return packages.ToList();
            }
        }

        public static async Task<List<NuGet.Versioning.NuGetVersion>> FindPackageVersionsAsync(string packageName, string packageSourceUrl)
        {
            List<string> packageNames = new List<string>();
            var cache = new SourceCacheContext();

            var nugetSource = new PackageSource(packageSourceUrl);
            SourceRepository repository = Repository.Factory.GetCoreV3(nugetSource);
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            using (var sourceCacheContext = new SourceCacheContext())
            {
                SearchFilter searchFilter = new SearchFilter(includePrerelease: false);
                var packages = await resource.GetAllVersionsAsync(packageName, cache, NullLogger.Instance, CancellationToken.None);

                return packages.ToList();
            }
        }

        public static async Task<bool> InstallPackageAsync(string packageId, string versionString, string packageSourceUrl, string filePath)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3(packageSourceUrl);
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            NuGetVersion packageVersion = new NuGetVersion(versionString);
            // write file stream
            using (var packageStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await resource.CopyNupkgToStreamAsync(
                    packageId,
                    packageVersion,
                    packageStream,
                    cache,
                    logger,
                    cancellationToken);
            }


            /*Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

            using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
            NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

            Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
            Console.WriteLine($"Description: {nuspecReader.GetDescription()}");*/
        }
    }
}
