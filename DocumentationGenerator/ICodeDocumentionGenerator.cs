using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.DocumentationGenerator
{
    public interface ICodeDocumentionGenerator
    {

        /// Sends C# code to the AI and returns the documented version.
        /// </summary>
        /// <param name="sourceCode">Original source code.</param>
        /// <returns>Documented source code.</returns>
        Task<string> GenerateDocumentationAsync(string sourceCode);
    }
}
