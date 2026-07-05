using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.DocumentationGenerator
{
    /// <summary>
    /// Scans directories and documents all C# files.
    /// </summary>
    public class DocumentationProcessor
    {

            private readonly ICodeDocumentionGenerator _generator;
            private  HashSet<string> _IgnoredDirectries = new HashSet<string>
            {
                    "bin",
                    "obj",
                    ".git",
                    ".vs"
            };


            private  HashSet<string> _IgnoredFiles = new HashSet<string>
            {
                    "AssemblyInfo.cs",
                    "Program.cs",
                    "Startup.cs",
                    ".Designer.cs",
                    ".g.cs",
            };

        public DocumentationProcessor(ICodeDocumentionGenerator generator, HashSet<string> IgnoredDirectries = null,HashSet<string> IgnoredFiles=null)
        {
           _generator = generator;
           if (IgnoredDirectries != null)
           { 
                foreach (var dir in IgnoredDirectries)
                {
                  _IgnoredDirectries.Add(dir);
                }
           }

            if (IgnoredFiles != null)
            {
                foreach (var file in IgnoredFiles)
                {
                    _IgnoredFiles.Add(file);
                }

            }
            
            
               

        }

            /// <summary>
            /// Scans the directory recursively and documents every .cs file.
            /// </summary>
            public async Task ProcessDirectoryAsync(string rootDirectory, IProgress<DocumentationProgress>? progress = null)
            {
                if (!Directory.Exists(rootDirectory))
                    throw new DirectoryNotFoundException(rootDirectory);


                // Get all .cs files, excluding ignored directories and files
                List<string> files = Directory
                    .EnumerateFiles(rootDirectory, "*.cs", SearchOption.AllDirectories).Where(file=>
                    {
                        string path = file.Replace('\\', '/');
                    if (_IgnoredDirectries.Any(dir =>
                    path.Contains("/" + dir + "/")))
                        return false;

                    if (_IgnoredFiles.Any(file.EndsWith))
                            return false;
                    return true;
                    })
                    .ToList();

                int processedFiles = 0;
                Console.WriteLine($"Found {files.Count} C# files.");

                foreach (string file in files)
                {
                    await ProcessFileAsync(file);
                    processedFiles++;
                    progress?.Report(new DocumentationProgress
                    {
                        ProcessedFiles = processedFiles,
                        TotalFiles = files.Count,
                        CurrentFile = file
                    });
            }

                Console.WriteLine("Documentation completed.");
            }

            private async Task ProcessFileAsync(string filePath)
            {
                try
                {
                    Console.WriteLine($"Processing: {Path.GetFileName(filePath)}");

                    string sourceCode =  File.ReadAllText(filePath);

                    if (string.IsNullOrWhiteSpace(sourceCode))
                    {
                        Console.WriteLine("Skipped (empty file)");
                        return;
                    }

                    string documentedCode =
                        await _generator.GenerateDocumentationAsync(sourceCode);

                    if (string.IsNullOrWhiteSpace(documentedCode))
                    {
                        Console.WriteLine("AI returned empty result.");
                        return;
                    }

                     File.WriteAllText(filePath, documentedCode);

                    Console.WriteLine("Done");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {filePath}");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

