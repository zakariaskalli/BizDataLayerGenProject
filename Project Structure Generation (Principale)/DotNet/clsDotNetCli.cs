using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet
{
    public sealed class clsDotNetCli
    {
        public static async Task ExecuteAsync(
            string arguments,
            string workingDirectory)
        {
            using var process = new Process();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            process.Start();

            var outputTask =
                process.StandardOutput.ReadToEndAsync();

            var errorTask =
                process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                $"dotnet command failed.\n\n" +
                $"Arguments: {arguments}\n\n" +
                $"Exit Code: {process.ExitCode}\n\n" +
                $"Output:\n{output}\n\n" +
                $"Error:\n{error}");
            }
        }
    }
}
