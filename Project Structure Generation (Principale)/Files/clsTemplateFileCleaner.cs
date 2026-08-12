using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BizDataLayerGen.Utils.clsProjectPathHelper;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Files
{
    public class clsTemplateFileCleaner
    {

        public static Task RemoveTemplateFilesAsync(
    SolutionConfiguration configuration, IProgress<ProjectStructureGenerationProgress> progress)
        {
            var filesToRemove = new[]
            {
                GetProjectFilePath(configuration, "API", "WeatherForecast.cs"),
                GetProjectFilePath(configuration, "API", "WeatherForecastController.cs"),
                GetProjectFilePath(configuration, "Business", "Class1.cs"),
                GetProjectFilePath(configuration, "DataAccess", "Class1.cs"),
                GetProjectFilePath(configuration, "DTO", "Class1.cs"),
                GetProjectFilePath(configuration, "Migrations", "Class1.cs")
    };

            foreach (var file in filesToRemove)
            {
                DeleteFileIfExists(file);
            }

            progress?.Report(new ProjectStructureGenerationProgress
            {
                ProcessedSteps = 7,
                CurrentStep = "Template files removed.",
                TotalSteps = 9,
            });
            return Task.CompletedTask;
        }

        private static void DeleteFileIfExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
            {
                throw new InvalidOperationException($"Failed to delete file '{filePath}'.", ex);
            }
        }

    }
}
