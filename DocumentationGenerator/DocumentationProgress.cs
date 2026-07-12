using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.DocumentationGenerator
{
    public class DocumentationProgress
    {
        public int ProcessedFiles { get; set; }
        public int TotalFiles { get; set; }

        public string CurrentFile { get; set; }  = string.Empty;

        public double Percentage => TotalFiles == 0 ? 0 : (double)ProcessedFiles / TotalFiles * 100;
    }
}
