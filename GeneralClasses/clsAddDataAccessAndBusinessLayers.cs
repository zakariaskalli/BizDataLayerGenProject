using BizDataLayerGen.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;



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

                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;
                
                var modifiedMessage = ex.Message + ", We have another Folders By The Same Name";
                var modifiedEx = new Exception(modifiedMessage, ex); // Create a new Exception with the modified message

                ErrorHandler.RaiseError(modifiedEx, className, methodName);
                
                return false;
            }

            return true;
        }

        public static bool CreateDataAccessSettingsClassFile(string ProjectName)
        {
            string errorHandlerFolderPath = Path.Combine(clsGlobal.dataAccessLayerPath, "ConnectionString");

            // Step 2: Create the folder if it doesn't exist
            if (!Directory.Exists(errorHandlerFolderPath))
            {
                Directory.CreateDirectory(errorHandlerFolderPath);
            }
            else
            {
            }

            // Step 3: Define the path for the 'clsErrorHandlingManager.cs' file
            string fullPath = Path.Combine(errorHandlerFolderPath, $"clsDataAccessSettings.cs");


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

        public static bool CreateSp_HandlerError()
        {
            bool isGood = false;

            // SQL query to check if the stored procedure already exists and create it if it doesn't
            string createProcedureQuery = $@"
USE [{clsGlobal.DataBaseName}];

-- Check if the stored procedure already exists
IF OBJECT_ID('SP_HandleError', 'P') IS NULL
BEGIN
    -- Create the stored procedure if it does not exist
    EXEC('CREATE PROCEDURE SP_HandleError
    AS
    BEGIN
        DECLARE @ErrorMessage NVARCHAR(4000), 
                @ErrorSeverity INT, 
                @ErrorState INT;

        -- Get error message, severity, and state
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Return the error details so they can be handled in the calling procedure
        SELECT 
            @ErrorMessage AS ErrorMessage,
            @ErrorSeverity AS ErrorSeverity,
            @ErrorState AS ErrorState;
    END');
END;
";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(createProcedureQuery, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery(); // Execute the command to check and create the procedure
                        isGood = true; // Set to true if execution succeeds
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // Example: Logger.Log(ex);
                isGood = false; // Set to false if an exception occurs
            }

            return isGood;
        }


        public static clsGlobal.enTypeRaisons AddDataAndBusinessLayers(string[] NameTables, bool FKOfAll, bool AddingStaticMethods )
        {
            Stopwatch stopwatch1 = Stopwatch.StartNew();

            if (NameTables == null)
            {
                return clsGlobal.enTypeRaisons.enError;
            }

            if (!CreateProjectFolders(clsGlobal.DataBaseName))
            {
                return clsGlobal.enTypeRaisons.enError;
            }

            if (!CreateDataAccessSettingsClassFile(clsGlobal.DataBaseName))
            {
                return clsGlobal.enTypeRaisons.enError;
            }

            if (!CreateDataAccessSettingsClassFile(clsGlobal.DataBaseName))
            {
                return clsGlobal.enTypeRaisons.enError;
            }

            if (!clsErrorHandling.CreatingForErrorHanding(clsGlobal.dataAccessLayerPath))
            {
                return clsGlobal.enTypeRaisons.enError;
            }

            if (!CreateSp_HandlerError())
            {
                return clsGlobal.enTypeRaisons.enError;
            }




            for (int i = 0; NameTables.Length > i; i++)
            {
                string[] Columns = clsGeneralWithData.GetColumnsName(NameTables[i], clsGlobal.DataBaseName);
                
                if (Columns == null)
                {
                    return clsGlobal.enTypeRaisons.enError;
                }

                string[] DataTypes = clsGeneralWithData.GetDataTypes(NameTables[i], clsGlobal.DataBaseName);

                if (DataTypes == null)
                {
                    return clsGlobal.enTypeRaisons.enError;
                }

                DataTypes = clsGenDataBizLayerMethods.ConvertSqlDataTypesToCSharp(DataTypes);


                bool[] NullibietyColumns = clsGeneralWithData.GetColumnNullabilityFromTable(NameTables[i], clsGlobal.DataBaseName);


                clsCreateDataAccessFile AddDataAccessLayer = new clsCreateDataAccessFile(clsGlobal.dataAccessLayerPath, NameTables[i], Columns, DataTypes, NullibietyColumns);

                clsGlobal.enTypeRaisons enRaisonForProjectDataAccess = AddDataAccessLayer.CreateDataAccessClassFile();




                // Build the path to the new folder "SPTables"
                string spTablesFolderPath = Path.Combine(clsGlobal.dataAccessLayerPath, "SPTables");

                // Ensure the folder exists (create it if it doesn't)
                if (!Directory.Exists(spTablesFolderPath))
                {
                    Directory.CreateDirectory(spTablesFolderPath);
                }

                string[] DataTypesForCreating = clsGeneralWithData.GetDataTypesForCreating(NameTables[i], clsGlobal.DataBaseName);
                // Create the instance of clsCreatingSPsForTable with the new folder path
                clsCreatingSPsForTable AddSPs = new clsCreatingSPsForTable(spTablesFolderPath, NameTables[i], Columns, DataTypesForCreating, NullibietyColumns);

                // Generate the stored procedures
                clsGlobal.enTypeRaisons enRaisonForProjectSPs = AddSPs.GenerateAllSPs();





                // Test "GetForeignKeys"

                // handle types of error enRaison and return
                if (enRaisonForProjectDataAccess != clsGlobal.enTypeRaisons.enPerfect)
                {
                    return enRaisonForProjectDataAccess;
                }

                string[] _ColumnNamesHasFK = { };
                string[] _TablesNameHasFK = { };
                string[] _ReferencedColumn = { };

                clsGeneralWithData.GetForeignKeysByTableName(NameTables[i], NameTables, clsGlobal.DataBaseName, FKOfAll, ref _ColumnNamesHasFK,ref _TablesNameHasFK, ref _ReferencedColumn);

                
                clsCreateBusinessLayerFile AddBusinessAccessLayer = new clsCreateBusinessLayerFile(clsGlobal.businessLayerPath, NameTables[i], Columns,
                    DataTypes, NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK, _ReferencedColumn, AddingStaticMethods);

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



            stopwatch1.Stop();
            clsGlobal.TimeInMillisecond = stopwatch1.ElapsedMilliseconds.ToString();

            
            return clsGlobal.enTypeRaisons.enPerfect;
        }
    }
}
