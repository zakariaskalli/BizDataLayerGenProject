// dotnet add package NuGet.Protocol
// dotnet add package NuGet.Frameworks   (usually pulled in transitively by NuGet.Protocol)
using NuGet.Frameworks;
using NuGet.Protocol;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Projects
{
    public static class NuGetVersionResolver
    {
        // <summary>
        /// Finds the highest package version that is compatible with the given target framework,
        /// using the same framework-compatibility engine NuGet/dotnet restore uses internally.
        /// </summary>
        /// <param name="packageId">e.g. "Newtonsoft.Json"</param>
        /// <param name="targetFrameworkMoniker">e.g. "net8.0", "net472", "netstandard2.0"</param>
        /// <param name="includePrerelease">whether to consider prerelease versions</param>
        public static async Task<decimal> GetBestCompatibleVersionAsync(
            string packageId,
            string targetFrameworkMoniker,
            bool includePrerelease = false)
        {
            var targetFramework = NuGetFramework.ParseFolder(targetFrameworkMoniker);
            var logger = NullLogger.Instance;
            var cache = new SourceCacheContext();

            // Official nuget.org v3 feed. Swap for a private feed URL if DealPart uses one.
            var repository = Repository.CreateSource(Repository.Provider.GetCoreV3(), "https://api.nuget.org/v3/index.json");
            var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>();

            var allVersions = await metadataResource.GetMetadataAsync(
                packageId,
                includePrerelease: includePrerelease,
                includeUnlisted: false,
                cache,
                logger,
                CancellationToken.None);

            var reducer = new FrameworkReducer();

            NuGetVersion? best = null;

            foreach (var package in allVersions)
            {
                // Each package version exposes the frameworks its dependency groups target.
                var supportedFrameworks = package.DependencySets
                    .Select(ds => ds.TargetFramework)
                    .Where(f => f != null)
                    .ToList();

                // No dependency groups usually means the package is framework-agnostic (e.g. content-only).
                var isCompatible = !supportedFrameworks.Any()
                    || reducer.GetNearest(targetFramework, supportedFrameworks) != null;

                if (!isCompatible) continue;

                var version = (NuGetVersion)package.Identity.Version;
                if (best is null || version > best)
                    best = version;
            }

            // NuGetVersion is semver-shaped (Major.Minor.Patch.Revision + optional prerelease
            // label, e.g. 8.2.1-preview.3). decimal can only hold two segments cleanly, so we
            // collapse to Major.Minor here. Patch/revision/prerelease info is lost — if you need
            // it, keep returning NuGetVersion instead and convert at the call site as needed.
            return best is null ? 0 : ToDecimal(best);
        }

        private static decimal ToDecimal(NuGetVersion version)
        {
            // e.g. 8.2.1 -> 8.2m
            return decimal.Parse($"{version.Major}.{version.Minor}");
        }
    }

    // Example usage:
    // var version = await NuGetVersionResolver.GetBestCompatibleVersionAsync("Serilog.AspNetCore", "net8.0");
    // Console.WriteLine(version is null ? "No compatible version found" : $"Best version: {version}");

}