using BizDataLayerGen.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.Creating_MigrationLayer
{
    public class clsCreateMigrationLayer
    {

        private string _filePath;
        clsCreateMigrationLayer(string filePath)
        {
            _filePath = filePath;
        }



        public async Task<clsGlobal.enTypeRaisons> CreateMigrationLayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"clsDbMigrator.cs");

            string code = @$"
using DbUp;
using {clsGlobal.ProjectName}_Shared;
using System.Reflection;


namespace {clsGlobal.ProjectName}_Migrations
{{


    public class clsDbMigrator
    {{
        public static void Migrate()
        {{
            EnsureDatabase.For.SqlDatabase(clsDataAccessSettings.ConnectionString, null);

            // Get current DLL directory
            string dllLocation = Assembly.GetExecutingAssembly().Location;
            string dllDirectory = Path.GetDirectoryName(dllLocation);

            // Go up to src folder
            string projectDir = Path.GetFullPath(Path.Combine(dllDirectory, @""..\..\..\..""));

            // Point to correct Migrations path inside DVLD_DataAccess
            string migrationsPath = Path.Combine(projectDir, ""DealPart_Migrations"", ""Migrations"");



            if (!Directory.Exists(migrationsPath))
                Directory.CreateDirectory(migrationsPath);

            var upgrader = DeployChanges.To
                .SqlDatabase(clsDataAccessSettings.ConnectionString)
                .WithScriptsFromFileSystem(migrationsPath)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                throw new Exception(""Database migration failed"", result.Error);
            }}

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(""Database migration successful!"");
            Console.ResetColor();
        }}
    }}
}}


";



           

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }


        public static async Task<clsGlobal.enTypeRaisons> CreateMigrationLayer(string filePath)
        {
            CreateMigrationFolder(filePath);
            clsCreateMigrationLayer migrationLayerCreator = new clsCreateMigrationLayer(filePath);
            return await migrationLayerCreator.CreateMigrationLayerFile();
        }

        public  clsGlobal.enTypeRaisons CreateMigrationFolder()
        {

            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.Combine(_filePath,"Migrations"));
            }

            return clsGlobal.enTypeRaisons.enPerfect;


        }
        public static clsGlobal.enTypeRaisons CreateMigrationFolder(string filePath)
        {
            if (!Directory.Exists(Path.Combine(filePath, "Migrations")))
            {
                Directory.CreateDirectory(Path.Combine(filePath, "Migrations"));
            }
            return clsGlobal.enTypeRaisons.enPerfect;
        }

    }
}
