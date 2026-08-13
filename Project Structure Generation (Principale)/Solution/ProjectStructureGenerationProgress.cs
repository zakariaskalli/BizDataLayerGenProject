using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.Solution
{
    public class ProjectStructureGenerationProgress
    {

        public int ProcessedSteps { get; set; }
        public int TotalSteps { get; set; }

        public string CurrentStep { get; set; } = string.Empty;

        public double Percentage => TotalSteps == 0 ? 0 : (double)ProcessedSteps / TotalSteps * 100;
    }
}
