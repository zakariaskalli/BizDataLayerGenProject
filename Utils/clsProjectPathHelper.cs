using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.Utils
{
    public static class clsProjectPathHelper
    {
        public static string GetProjectFilePath(
            SolutionConfiguration configuration,
            string projectLayer,
            string fileName)
        {
            var projectName =
                $"{configuration.SolutionName}_{projectLayer}";

            return Path.Combine(
                configuration.OutputDirectory,
                projectName,
                fileName);
        }

        public static string GetProjectPath(string SolutionName, string projectLayer,string Directory)
        {
            var projectName =
                $"{SolutionName}_{projectLayer}";

            return Path.Combine(
                Directory,
                projectName);
        }

    }
}
