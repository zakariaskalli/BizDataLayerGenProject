using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Solution
{
    public class clsSolutionGenerator
    {

  
        public static async Task GenerateSolutionAsync(SolutionConfiguration configuration)
        {
            ValidateConfiguration(configuration);

            await CreateSolutionAsync(configuration);

            await clsProjectCreator.CreateProjectsAsync(configuration);

            await clsProjectRefrenceManager.AddProjectsToSolutionAsync(configuration);

            await clsProjectRefrenceManager. AddProjectReferencesAsync(configuration);

            await clsNuGetPackageManager.InstallRequiredPackagesAsync(configuration);

            await Files.clsTemplateFileCleaner.RemoveTemplateFilesAsync(configuration);

            await RestorePackagesAsync(configuration);

            await BuildSolutionAsync(configuration);
        }


        private static void ValidateConfiguration(SolutionConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.SolutionName))
                throw new ArgumentException(
                    "Solution name cannot be empty.",
                    nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.OutputDirectory))
                throw new ArgumentException(
                    "Output directory cannot be empty.",
                    nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.DotNetVersion))
                throw new ArgumentException(
                    ".NET version cannot be empty.",
                    nameof(configuration));
        }

        private static async Task CreateSolutionAsync(
        SolutionConfiguration configuration)
        {
            await clsDotNetCli.ExecuteAsync(
                $"new sln --name \"{configuration.SolutionName}\" --format sln",
                configuration.OutputDirectory);
        }

        public static string GetSolutionPath(
        SolutionConfiguration configuration)
        {
            return Path.Combine(
                configuration.OutputDirectory,
                $"{configuration.SolutionName}.sln");
        }


        private static async Task BuildSolutionAsync(
        SolutionConfiguration configuration)
        {
            var solutionPath = GetSolutionPath(configuration);

            await clsDotNetCli.ExecuteAsync(
                $"build \"{solutionPath}\" --no-restore",
                configuration.OutputDirectory);

        }


        private static async Task RestorePackagesAsync(
        SolutionConfiguration configuration)
        {
            var solutionPath = GetSolutionPath(configuration);

            await clsDotNetCli.ExecuteAsync(
                $"restore \"{solutionPath}\"",
                configuration.OutputDirectory);
        }
    }
}
