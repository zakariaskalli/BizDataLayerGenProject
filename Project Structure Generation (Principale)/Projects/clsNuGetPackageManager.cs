using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BizDataLayerGen.Utils;
using System.Threading.Tasks;

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
                    "Swashbuckle.AspNetCore", "8.1.0");
            }

            if (configuration.EnableApiVersioning)
            {
                await AddPackageAsync(
                    $"{projectPath}\\{configuration.SolutionName}_API.csproj",
                    "Asp.Versioning.Mvc", "8.1.0");

                await AddPackageAsync(
                    $"{projectPath}\\{configuration.SolutionName}_API.csproj",
                    "Asp.Versioning.Mvc.ApiExplorer", "8.1.0");
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
                "Microsoft.Data.SqlClient", "7.0.2");
            await AddPackageAsync(
               $"{projectPath}\\{configuration.SolutionName}_DataAccess.csproj",
               "Newtonsoft.Json", null);
        }

        private static async Task InstallMigrationPackagesAsync(SolutionConfiguration configuration)
        {
            var projectPath =clsProjectPathHelper.GetProjectPath(
                configuration.SolutionName,
                "Migrations",configuration.OutputDirectory);


            await AddPackageAsync(
                $"{projectPath}\\{configuration.SolutionName}_Migrations.csproj",
                "DbUp", "5.0.40");


            await AddPackageAsync(
                $"{projectPath}\\{configuration.SolutionName}_Migrations.csproj",
                "Microsoft.Data.SqlClient", "7.0.2");


        }



        private static async Task AddPackageAsync(
            string projectFilePath,
            string packageName,string version = null)
        {
            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException(
                    $"Project file not found: {projectFilePath}");
            }

            string command =
            $"add \"{projectFilePath}\" package {packageName}" +
            $"{(!string.IsNullOrWhiteSpace(version) ? $" --version {version}" : "")}";

            await clsDotNetCli.ExecuteAsync(
                command,
                Path.GetDirectoryName(projectFilePath));
        }

    }
}
