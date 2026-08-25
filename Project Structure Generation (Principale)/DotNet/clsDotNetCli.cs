using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BizDataLayerGen.Project_Structure_Generation__Principale_.DotNet
{
    public sealed class clsDotNetCli
    {
        public static async Task ExecuteAsync(string arguments, string workingDirectory)
        {
            
            using var process = new Process();

            try
            {
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

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // جعل العملية تنتظر انتهاء الـ Process بدون إغلاق الـ UI thread
                await Task.Run(() => process.WaitForExit());

                var output = await outputTask;
                var error = await errorTask;

                if (process.ExitCode != 0)
                {

                    // exit code 127  is commonlt associated with a missing command
                    if (process.ExitCode == 127 ||
                    error.Contains("SDK") ||
                    error.Contains("not installed"))
                    {
                        throw new InvalidOperationException(
                            $"The required .NET SDK could not be found.\n\n" +
                            $"Command: dotnet {arguments}\n\n" +
                            $"Please install the required .NET SDK or select a " +
                            $"version that is already installed on your machine.\n\n" +
                            $"Details:\n{error}");
                    }


                    throw new InvalidOperationException(
                        $"dotnet command failed.\n\n" +
                        $"Arguments: {arguments}\n\n" +
                        $"Exit Code: {process.ExitCode}\n\n" +
                        $"Output:\n{output}\n\n" +
                        $"Error:\n{error}");
                }
            }
            catch(Exception ex) 
            {
                throw new InvalidOperationException
                    ($"Failed to execute the dotnet CLI.\n\n" +
                    $"Arguments: {arguments}\n\n" +
                    $"Working Directory: {workingDirectory}\n\n" +
                    $"Error: {ex.Message}",
                    ex);
            }
            
        }


        public static bool IsDotNetSdkInstalled(string targetFrameworkMoniker)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--list-sdks",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processStartInfo);

                if (process == null)
                    return false;

                var output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                    return false;

                // Convert "net9.0" → "9.0"
                var version = targetFrameworkMoniker.Replace("net", "");

                // Check SDKs such as:
                // 9.0.304
                // 9.0.305
                // 10.0.100
                return output
                    .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)
                    .Any(line => line.Trim().StartsWith(version + '.',StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}