using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using BizDataLayerGen.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text.Json;
using System.Globalization;
using NuGet.Frameworks;

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



        private static async Task AddPackageAsync(
            string projectFilePath,
            string packageName,string targetFrameworkversion)
        {
            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException(
                    $"Project file not found: {projectFilePath}");
            }

            string command =
            $"add \"{projectFilePath}\" package {packageName}" +
            $" --version {await NuGetVersionResolver.GetBestCompatibleVersionAsync(packageName,targetFrameworkversion)}";


            await clsDotNetCli.ExecuteAsync(
                command,
                Path.GetDirectoryName(projectFilePath));
        }
    }
}
