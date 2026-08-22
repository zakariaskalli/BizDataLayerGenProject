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




        public static async Task GenerateSolutionAsync(SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress = null)
        {
            int CurrentStep = 0;
            int TotalSteps =  9;

            async Task ReportAsync(string description)
            {
                progress?.Report(new ProjectStructureGenerationProgress
                {
                    CurrentStep = description,
                    ProcessedSteps = CurrentStep,
                    TotalSteps = TotalSteps,
                });
                await Task.Yield();
            }

            await ReportAsync("Starting solution generation...");

            ValidateConfiguration(configuration);
            CurrentStep++;
            await ReportAsync("Configuration validated successfully.");

            await CreateSolutionAsync(configuration);
            CurrentStep++;
            await ReportAsync("Solution created successfully.");

            await clsProjectCreator.CreateProjectsAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Projects created successfully.");

            await clsProjectRefrenceManager.AddProjectsToSolutionAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Projects added to solution.");

            await clsProjectRefrenceManager.AddProjectReferencesAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Project references added.");


            await Files.clsTemplateFileCleaner.RemoveTemplateFilesAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Template files removed.");


            await clsNuGetPackageManager.InstallRequiredPackagesAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("NuGet packages installed.");

            await RestorePackagesAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Packages restored.");

            await BuildSolutionAsync(configuration, progress);
            CurrentStep++;
            await ReportAsync("Solution built successfully.");
        }


        private static void ValidateConfiguration(SolutionConfiguration configuration,IProgress<ProjectStructureGenerationProgress> progress = null)
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

            progress?.Report( new ProjectStructureGenerationProgress
            {
                CurrentStep = "Configuration validated successfully.",
                ProcessedSteps = 1,
                TotalSteps = 9,
            });
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
        SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress=null)
        {
            var solutionPath = GetSolutionPath(configuration);

            await clsDotNetCli.ExecuteAsync(
                $"build \"{solutionPath}\" --no-restore",
                configuration.OutputDirectory);

            progress?.Report(new ProjectStructureGenerationProgress
            {
                ProcessedSteps = 9,
                CurrentStep = "Solution built successfully.",
                TotalSteps = 9,
            });

        }

        

        private static async Task RestorePackagesAsync(
        SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress=null)
        {
            var solutionPath = GetSolutionPath(configuration);

            await clsDotNetCli.ExecuteAsync(
                $"restore \"{solutionPath}\"",
                configuration.OutputDirectory);

            progress?.Report(new ProjectStructureGenerationProgress
            {
                ProcessedSteps = 8,
                CurrentStep = "NuGet packages restored successfully.",
                TotalSteps = 9,
            });
        }
    }
}
