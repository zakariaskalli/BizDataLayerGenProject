using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using System.Threading;
using System.Threading.Tasks;
using static BizDataLayerGen.Project_Structure_Generation__Principale_.Projects.clsProjectRefrenceManager;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Projects
{
    public class clsProjectCreator
    {
        public static async Task CreateProjectsAsync(
        SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress)
        {
            if (configuration.IncludeApi)
            {
                await CreateApiProjectAsync(
                    configuration);
            }

            if (configuration.IncludeBusiness)
            {
                await CreateBusinessProjectAsync(
                    configuration);
            }

            if (configuration.IncludeDataAccess)
            {
                await CreateDataAccessProjectAsync(
                    configuration);
            }

            if (configuration.IncludeDto)
            {
                await CreateDtoProjectAsync(
                    configuration);
            }

            if (configuration.IncludeMigrations)
            {
                await CreateMigrationsProjectAsync(
                    configuration);
            }

            progress?.Report(new ProjectStructureGenerationProgress
            {
                ProcessedSteps = 4,
                CurrentStep = "Project creation completed.",TotalSteps = 9,
            });
        }


        private static async Task CreateApiProjectAsync(
        SolutionConfiguration configuration)
        {
            var projectName = $"{configuration.SolutionName}_API";

            await clsDotNetCli.ExecuteAsync(
                $"new webapi -n \"{projectName}\" -f {configuration.DotNetVersion}",
                configuration.OutputDirectory);

        }

        private static async Task CreateBusinessProjectAsync(
        SolutionConfiguration configuration)
        {
            var projectName = $"{configuration.SolutionName}_Business";

            await clsDotNetCli.ExecuteAsync(
                $"new classlib -n \"{projectName}\" -f {configuration.DotNetVersion}",
                configuration.OutputDirectory);
        }

        private static async Task CreateDataAccessProjectAsync(
        SolutionConfiguration configuration)
        {
            var projectName = $"{configuration.SolutionName}_DataAccess";

            await clsDotNetCli.ExecuteAsync(
                $"new classlib -n \"{projectName}\" -f {configuration.DotNetVersion}",
                configuration.OutputDirectory);
        }


        private static async Task CreateDtoProjectAsync(
        SolutionConfiguration configuration)
        {
            var projectName = $"{configuration.SolutionName}_DTO";

            await clsDotNetCli.ExecuteAsync(
                $"new classlib -n \"{projectName}\" -f {configuration.DotNetVersion}",
                configuration.OutputDirectory
                );
        }


        private static async Task CreateMigrationsProjectAsync(
        SolutionConfiguration configuration)
        {
            var projectName = $"{configuration.SolutionName}_Migrations";

            await clsDotNetCli.ExecuteAsync(
                $"new classlib -n \"{projectName}\" -f {configuration.DotNetVersion}",
                configuration.OutputDirectory);
        }


        private static GeneratedProject CreateProjectInfo(
    SolutionConfiguration configuration,
    string layer)
        {
            var projectName =
                $"{configuration.SolutionName}_{layer}";

            var projectPath = Path.Combine(
                configuration.OutputDirectory,
                projectName,
                $"{projectName}.csproj");

            return new GeneratedProject(
                projectName,
                projectPath);
        }

        public static IReadOnlyCollection<GeneratedProject> GetProjects(
   SolutionConfiguration configuration)
        {
            var projects = new List<GeneratedProject>();

            if (configuration.IncludeApi)
            {
                projects.Add(CreateProjectInfo(
                    configuration,
                    "API"));
            }

            if (configuration.IncludeBusiness)
            {
                projects.Add(CreateProjectInfo(
                    configuration,
                    "Business"));
            }

            if (configuration.IncludeDataAccess)
            {
                projects.Add(CreateProjectInfo(
                    configuration,
                    "DataAccess"));
            }

            if (configuration.IncludeDto)
            {
                projects.Add(CreateProjectInfo(
                    configuration,
                    "DTO"));
            }

            if (configuration.IncludeMigrations)
            {
                projects.Add(CreateProjectInfo(
                    configuration,
                    "Migrations"));
            }

            return projects;
        }




    }





    public class GeneratedProject
    {
        public string Name { get; }
        public string ProjectPath { get; }

        public GeneratedProject(string name, string projectPath)
        {
            Name = name;
            ProjectPath = projectPath;
        }
    }



}

