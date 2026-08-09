using BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BizDataLayerGen.Utils.clsProjectPathHelper;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Projects
{
    public class clsProjectRefrenceManager
    {
        public static async Task AddProjectsToSolutionAsync(
            SolutionConfiguration configuration)
        {
            var solutionPath = clsSolutionGenerator.GetSolutionPath(configuration);

            foreach (var project in clsProjectCreator.GetProjects(configuration))
            {
                await clsDotNetCli.ExecuteAsync(
                    $"sln \"{solutionPath}\" add \"{project.ProjectPath}\"",
                    configuration.OutputDirectory
                    );
            }
        }

        public static async Task AddProjectReferencesAsync(
        SolutionConfiguration configuration)
        {
            if (configuration.IncludeApi &&
                configuration.IncludeBusiness)
            {
                await AddProjectReferenceAsync(
                    configuration,
                    "API",
                    "Business");
            }


            // add api refrence dto layer
            if (configuration.IncludeApi &&
               configuration.IncludeDto)
            {
                await AddProjectReferenceAsync(
                    configuration,
                    "API",
                    "DTO");
            }

            if (configuration.IncludeBusiness &&
                configuration.IncludeDataAccess)
            {
                await AddProjectReferenceAsync(
                    configuration,
                    "Business",
                    "DataAccess");
            }

            if (configuration.IncludeBusiness &&
                configuration.IncludeDto)
            {
                await AddProjectReferenceAsync(
                    configuration,
                    "Business",
                    "DTO");
            }

            if (configuration.IncludeDataAccess &&
                configuration.IncludeDto)
            {
                await AddProjectReferenceAsync(
                    configuration,
                    "DataAccess",
                    "DTO");
            }

            //if (configuration.IncludeMigrations &&
            //    configuration.IncludeDataAccess)
            //{
            //    await AddProjectReferenceAsync(
            //        configuration,
            //        "Migrations",
            //        "DataAccess");
            //}
        }

        private static async Task AddProjectReferenceAsync(
    SolutionConfiguration configuration,
    string sourceProject,
    string targetProject)
        {
            var sourcePath = GetProjectPath(
                configuration.SolutionName,
                sourceProject,configuration.OutputDirectory);

            var targetPath = GetProjectPath(
                configuration.SolutionName,
                targetProject,configuration.OutputDirectory);

            await clsDotNetCli.ExecuteAsync(
                $"add \"{sourcePath}\" reference \"{targetPath}\"",
                configuration.OutputDirectory);
        }

    }
}
