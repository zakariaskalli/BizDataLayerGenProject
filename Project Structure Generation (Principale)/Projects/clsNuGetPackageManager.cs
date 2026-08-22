using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using BizDataLayerGen.Utils;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Projects
{
    public class clsNuGetPackageManager
    {
        public static async Task InstallRequiredPackagesAsync(
        SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress)
        {
            await InstallApiPackagesAsync(
                configuration);

            await InstallDataAccessPackagesAsync(
                configuration);

            await InstallMigrationPackagesAsync(
                configuration);
            progress?.Report(new ProjectStructureGenerationProgress
            {
                ProcessedSteps = 6,
                CurrentStep = "NuGet package installation completed.",
                TotalSteps = 9,
            });
        }

        private static async Task InstallApiPackagesAsync(
    SolutionConfiguration configuration)
        {

           
            var projectPath = clsProjectPathHelper.GetProjectPath(
                configuration.SolutionName,
                "API",configuration.OutputDirectory);

            if (configuration.EnableSwagger)
            {
                await AddPackageAsync(
                    $"{projectPath}\\{configuration.SolutionName}_API.csproj",
                     "Swashbuckle.AspNetCore", $"net{configuration.DotNetVersion}");
            }

            if (configuration.EnableApiVersioning)
            {
                await AddPackageAsync(
                    $"{projectPath}\\{configuration.SolutionName}_API.csproj",
                    "Asp.Versioning.Mvc", $"net{configuration.DotNetVersion}");

                await AddPackageAsync(
                    $"{projectPath}\\{configuration.SolutionName}_API.csproj",
                    "Asp.Versioning.Mvc.ApiExplorer", $"net{configuration.DotNetVersion}");
            }
        }

        private static async Task InstallDataAccessPackagesAsync(SolutionConfiguration configuration)
        {
            var projectPath = clsProjectPathHelper. GetProjectPath(
                configuration.SolutionName,
                "DataAccess",
                configuration.OutputDirectory
                );
            await AddPackageAsync(
                $"{projectPath}\\{configuration.SolutionName}_DataAccess.csproj",
                "Microsoft.Data.SqlClient", $"net{configuration.DotNetVersion}");
            await AddPackageAsync(
               $"{projectPath}\\{configuration.SolutionName}_DataAccess.csproj",
               "Newtonsoft.Json", $"net{configuration.DotNetVersion}");
        }

        private static async Task InstallMigrationPackagesAsync(SolutionConfiguration configuration)
        {
            var projectPath =clsProjectPathHelper.GetProjectPath(
                configuration.SolutionName,
                "Migrations",configuration.OutputDirectory);


            await AddPackageAsync(
                $"{projectPath}\\{configuration.SolutionName}_Migrations.csproj",
                "DbUp",$"net{configuration.DotNetVersion}");


            await AddPackageAsync(
                $"{projectPath}\\{configuration.SolutionName}_Migrations.csproj",
                "Microsoft.Data.SqlClient", $"net{configuration.DotNetVersion}");


        }


        public static void AddPackageReference(string csprojPath, string packageName, string version)
        {
            var doc = XDocument.Load(csprojPath);

            // Find an existing ItemGroup that already contains PackageReference elements,
            // or create a new one if none exists
            var itemGroup = doc.Root
                .Elements("ItemGroup")
                .FirstOrDefault(ig => ig.Elements("PackageReference").Any());

            if (itemGroup == null)
            {
                itemGroup = new XElement("ItemGroup");
                doc.Root.Add(itemGroup);
            }

            // Avoid adding a duplicate reference for the same package
            var existing = itemGroup.Elements("PackageReference")
                .FirstOrDefault(pr => (string)pr.Attribute("Include") == packageName);

            if (existing != null)
            {
                existing.SetAttributeValue("Version", version);
            }
            else
            {
                itemGroup.Add(new XElement("PackageReference",
                    new XAttribute("Include", packageName),
                    new XAttribute("Version", version)));
            }

            doc.Save(csprojPath);
        }

      

        private static async Task AddPackageAsync(
            string projectFilePath,
            string packageName,string targetFrameworkversion)
        {
            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException(
                    $"Project file not found: {projectFilePath}");
            }

           string packageVersion = await NuGetVersionResolver.GetBestCompatibleVersionAsync(packageName, targetFrameworkversion);
           string versionArg = string.IsNullOrEmpty(packageVersion)
           ? string.Empty
           : $"--version {packageVersion}";

           
           string command =
            $"add \"{projectFilePath}\" package {packageName}" +
            $"{versionArg}";


            try
            {
               await clsDotNetCli.ExecuteAsync(
               command,
               Path.GetDirectoryName(projectFilePath));
            }
            catch
            {
                 AddPackageReference(projectFilePath, packageName, packageVersion);
            }
           
        }
    }
}
