using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Projects
{
    public static class NuGetVersionResolver
    {
        // Session-level cache: key = "packageId|targetFramework|includePrerelease"
        private static readonly ConcurrentDictionary<string, string> VersionCache =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Finds the highest package version compatible with the target framework.
        /// If NuGet.org is unavailable, a predefined fallback version is returned.
        /// Results are cached in-memory for the duration of the application session.
        /// </summary>
        public static async Task<string> GetBestCompatibleVersionAsync(
            string packageId,
            string targetFrameworkMoniker,
            bool includePrerelease = false)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentException(
                    "Package ID cannot be empty.",
                    nameof(packageId));

            if (string.IsNullOrWhiteSpace(targetFrameworkMoniker))
                throw new ArgumentException(
                    "Target framework cannot be empty.",
                    nameof(targetFrameworkMoniker));

            var cacheKey = $"{packageId}|{targetFrameworkMoniker}|{includePrerelease}";

            if (VersionCache.TryGetValue(cacheKey, out var cachedVersion))
            {
                return cachedVersion;
            }

            var targetFramework =
                NuGetFramework.ParseFolder(targetFrameworkMoniker);

            if (targetFramework.IsUnsupported)
            {
                throw new ArgumentException(
                    $"Unsupported target framework: '{targetFrameworkMoniker}'.",
                    nameof(targetFrameworkMoniker));
            }

            IEnumerable<IPackageSearchMetadata> allVersions;
            string resolvedVersion;

            try
            {
                var logger = NullLogger.Instance;

                using var cache = new SourceCacheContext();

                var repository = Repository.CreateSource(
                    Repository.Provider.GetCoreV3(),
                    "https://api.nuget.org/v3/index.json");

                var metadataResource =
                    await repository.GetResourceAsync<PackageMetadataResource>();

                allVersions = await metadataResource.GetMetadataAsync(
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
                    var supportedFrameworks = package.DependencySets
                        .Select(ds => ds.TargetFramework)
                        .Where(f => f != null)
                        .ToList();

                    var isCompatible =
                        !supportedFrameworks.Any()
                        || reducer.GetNearest(
                            targetFramework,
                            supportedFrameworks) != null;

                    if (!isCompatible)
                        continue;

                    var version =
                        (NuGetVersion)package.Identity.Version;

                    if (best == null || version > best)
                    {
                        best = version;
                    }
                }

                resolvedVersion = best != null
                    ? best.ToString()
                    : FallbackVersions[packageId];
            }
            catch (HttpRequestException)
            {
                // Internet unavailable / NuGet.org unreachable.
                resolvedVersion = FallbackVersions[packageId];
            }
            catch (TaskCanceledException)
            {
                // Timeout while trying to reach NuGet.org.
                resolvedVersion = FallbackVersions[packageId];
            }

            VersionCache.TryAdd(cacheKey, resolvedVersion);
            return resolvedVersion;
        }

        private static readonly Dictionary<string, string> FallbackVersions =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Newtonsoft.Json", "12.0.3" },
                { "Swashbuckle.AspNetCore", "6.5.0" },
                { "Asp.Versioning.Mvc", "8.0.0" },
                { "Asp.Versioning.Mvc.ApiExplorer", "8.0.0" },
                { "Microsoft.Extensions.Configuration", "2.0.0" },
                { "Microsoft.Data.SqlClient", "4.0.0" },
                { "DbUp", "5.0.0" }
            };
    }
}