using BizDataLayerGen.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;



namespace BizDataLayerGen.GeneralClasses
{
    public class clsAddDataAccessAndBusinessLayers
    {
        
        public static bool CreateProjectFolders(string ProjectName)
        {

            try
            {
                string dataAccessLayerPath = "";
                string businessLayerPath = "";


                // Define folder names for Data Access Layer and Business Layer
                clsGlobal.dataAccessLayerPath = Path.Combine(clsGlobal.PathFilesToGenerate, ProjectName + "_DataAccess");
                clsGlobal.businessLayerPath = Path.Combine(clsGlobal.PathFilesToGenerate, ProjectName + "_Business");

                dataAccessLayerPath = clsGlobal.dataAccessLayerPath;
                businessLayerPath = clsGlobal.businessLayerPath;


                // Check if the folders already exist, if not, create them
                if (!Directory.Exists(dataAccessLayerPath))
                    Directory.CreateDirectory(dataAccessLayerPath);
                else
                    return false;

                if (!Directory.Exists(businessLayerPath))
                    Directory.CreateDirectory(businessLayerPath);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool CreateDataAccessSettingsClassFile(string ProjectName)
        {
            // Define the full path for the file
            string fullPath = Path.Combine(clsGlobal.dataAccessLayerPath, $"clsDataAccessSettings.cs");

            // Define the code to be written in the file
            string code = $@"
using System;
namespace {ProjectName}_DataAccess
{{
    static class clsDataAccessSettings
    {{
        public static string ConnectionString = ""Server=.;Database={clsGlobal.DataBaseName};User Id={clsGlobal.UserId};Password={clsGlobal.Password}"";


    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            string fileContent = File.ReadAllText(fullPath);

            if (fileContent.Contains("class") || fileContent.Contains("namespace") || fileContent.Contains("using"))
                return true;
            else
                return false;
        }

        public static clsGlobal.enTypeRaisons AddDataAndBusinessLayers(string[] NameTables)
        {


            if (NameTables == null)
            {
                return clsGlobal.enTypeRaisons.enDBName;
            }

            if (!CreateProjectFolders(clsGlobal.DataBaseName))
            {
                return clsGlobal.enTypeRaisons.enCreateProjectFolders;
            }

            if (!CreateDataAccessSettingsClassFile(clsGlobal.DataBaseName))
            {
                return clsGlobal.enTypeRaisons.enCreateDataAccessSettingsClassFile;
            }

            for (int i = 0; NameTables.Length > i; i++)
            {
                string[] Columns = clsGeneralWithData.GetColumnsName(NameTables[i], clsGlobal.DataBaseName);
                
                if (Columns == null)
                {
                    return clsGlobal.enTypeRaisons.enReloadColumnsName;
                }

                string[] DataTypes = clsGeneralWithData.GetDataTypes(NameTables[i], clsGlobal.DataBaseName);

                if (DataTypes == null)
                {
                    return clsGlobal.enTypeRaisons.enReloadDataTypes;
                }

                DataTypes = clsGenDataBizLayerMethods.ConvertSqlDataTypesToCSharp(DataTypes);


                bool[] NullibietyColumns = clsGeneralWithData.GetColumnNullabilityFromTable(NameTables[i], clsGlobal.DataBaseName);


                clsCreateDataAccessFile AddDataAccessLayer = new clsCreateDataAccessFile(clsGlobal.dataAccessLayerPath, NameTables[i], Columns, DataTypes, NullibietyColumns);

                clsGlobal.enTypeRaisons enRaisonForProjectDataAccess = AddDataAccessLayer.CreateDataAccessClassFile();

                // Test "GetForeignKeys"

                // handle types of error enRaison and return
                if (enRaisonForProjectDataAccess != clsGlobal.enTypeRaisons.enPerfect)
                {
                    return enRaisonForProjectDataAccess;
                }

                string[] _ColumnNamesHasFK = { };
                string[] _TablesNameHasFK = { };

                clsGeneralWithData.GetForeignKeysByTableName(NameTables[i], NameTables, clsGlobal.DataBaseName,ref _ColumnNamesHasFK,ref _TablesNameHasFK);


                clsCreateBusinessLayerFile AddBusinessAccessLayer = new clsCreateBusinessLayerFile(clsGlobal.businessLayerPath, NameTables[i], Columns, DataTypes, NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK);

                clsGlobal.enTypeRaisons enRaisonForProjectBusiness = AddBusinessAccessLayer.CreateBusinessLayerFile();


                if (enRaisonForProjectBusiness != clsGlobal.enTypeRaisons.enPerfect)
                {
                    return enRaisonForProjectBusiness;
                }


                //if (!clsGeneralWithData.HasForeignKey(NameTables[i]))
                //{
                //    
                //}
                //else
                //{
                //
                //}

            }



            /*
            string cscPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"; // تأكد من أن المسار صحيح أو ابحث عن المسار المناسب لإصدار .NET لديك
            string outputDllPath = Path.Combine(clsGlobal.dataAccessLayerPath, $"{clsGlobal.DataBaseName}.dll");

            string sourceFiles = string.Join(" ", Directory.GetFiles(clsGlobal.dataAccessLayerPath, "*.cs"));

            Process process = new Process();
            process.StartInfo.FileName = cscPath;
            process.StartInfo.Arguments = $"-target:library -out:{outputDllPath} {sourceFiles}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // تشغيل العملية
            process.Start();
            process.WaitForExit();
            */

            return clsGlobal.enTypeRaisons.enPerfect;
        }
    }
}
