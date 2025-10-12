using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Xcaciv.Command.Packages;

namespace Xcaciv.Command.PackagesTests
{
    public class NugetWrapperTests
    {
        [Fact]
        public async Task FindPackageAsync_ReturnsExpectedPackages()
        {
            // Arrange
            string searchTerm = "XCBatch";
            string packageSourceUrl = "https://api.nuget.org/v3/index.json";
            var nugetSource = new PackageSource(packageSourceUrl);
            SourceRepository repository = Repository.Factory.GetCoreV3(nugetSource);

            int limit = 20;
            bool prerelease = false;

            // Act
            var packages = await NugetWrapper.FindPackageAsync(searchTerm, repository, limit, prerelease);

            // Assert
            Assert.NotNull(packages);
            Assert.True(packages.Count > 0);
        }

        [Fact]
        public async Task FindPackageVersionsAsync_ReturnsExpectedVersions()
        {
            // Arrange
            string packageName = "XCBatch.Core";
            string packageSourceUrl = "https://api.nuget.org/v3/index.json";

            // Act
            var versions = await NugetWrapper.FindPackageVersionsAsync(packageName, packageSourceUrl);

            // Assert
            Assert.NotNull(versions);
            Assert.True(versions.Count > 0);
        }

        [Fact]
        public async Task DownloadPackageAsync_ReturnsTrue()
        {
            // Arrange
            string packageId = "XCBatch.Core";
            string versionString = "1.0.0";
            string packageSourceUrl = "https://api.nuget.org/v3/index.json";
            string filePath = @"E:\tmp\package.nupkg";

            var nugetSource = new PackageSource(packageSourceUrl);
            SourceRepository repository = Repository.Factory.GetCoreV3(nugetSource);

            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(versionString));

            // Act
            var result = await NugetWrapper.DownloadPackageAsync(packageIdentity, repository, filePath);

            // Assert
            Assert.True(result);
        }
    }
}
