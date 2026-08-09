using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Solution
{

        public sealed class SolutionConfiguration
        {
            public string SolutionName { get; set; }

            public string OutputDirectory { get; set; }

            public string DotNetVersion { get; set; }

            public bool IncludeApi { get; set; } = true;

            public bool IncludeBusiness { get; set; } = true;

            public bool IncludeDataAccess { get; set; } = true;

            public bool IncludeDto { get; set; } = true;

            public bool IncludeMigrations { get; set; } = true;

            public bool EnableSwagger { get; set; } = true;

            public bool EnableApiVersioning { get; set; } = true;

            public SolutionConfiguration() { }

            
            public SolutionConfiguration(string solutionName, string outputDirectory, string dotNetVersion)
            {
                SolutionName = solutionName;
                OutputDirectory = outputDirectory;
                DotNetVersion = dotNetVersion;
            }
        }
    }

