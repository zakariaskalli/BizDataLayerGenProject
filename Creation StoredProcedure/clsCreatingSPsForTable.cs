using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace BizDataLayerGen.DataAccessLayer
{
    public class clsCreatingSPsForTable
    {
        private string _filePath;
        private string _tableName;
        private string[] _columns;
        private string[] _dataTypes;
        private bool[] _nullabilityColumns;
        private bool AutoExcuteSP;

        public clsCreatingSPsForTable(string filePath, string tableName, string[] columns,
            string[] dataTypes, bool[] nullabilityColumns, bool autoExcuteSP)
        {
            this._filePath = filePath;
            this._tableName = tableName;
            this._columns = columns;
            this._dataTypes = dataTypes;
            this._nullabilityColumns = nullabilityColumns;
            AutoExcuteSP = autoExcuteSP;
        }

        public static void ExecuteSqlFile(string filePath)
        {
            string script = File.ReadAllText(filePath);

            // Split script on "GO" (case-insensitive) with trimming
            string[] commands = script
                .Split(new[] { "GO", "go", "Go", "gO" }, StringSplitOptions.RemoveEmptyEntries);

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                foreach (string cmd in commands)
                {
                    if (string.IsNullOrWhiteSpace(cmd)) continue;

                    using (SqlCommand sqlCmd = new SqlCommand(cmd, conn))
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public clsGlobal.enTypeRaisons GenerateAllSPs()
        {
            // Build the full path for the .sql file with the desired structure
            string fileName = $"{_tableName}_SPs.sql";
            string fullPath = Path.Combine(_filePath, fileName);

            // Use StringBuilder to combine all stored procedure scripts
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-- Stored Procedures for Table: " + _tableName);
            
            sb.AppendLine();
            sb.AppendLine($"Use [{clsGlobal.DataBaseName}];");
            sb.AppendLine("Go");
            sb.AppendLine();

            sb.AppendLine(CreateSP_GetByID());
            sb.AppendLine(CreateSP_GetAll());
            sb.AppendLine(CreateSP_Add());
            sb.AppendLine(CreateSP_UpdateByID());
            sb.AppendLine(CreateSP_DeleteByID());
            sb.AppendLine(CreateSP_SearchByColumn());

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write all SPs to the dynamically generated .sql file
            File.WriteAllText(fullPath, sb.ToString());

            if (AutoExcuteSP)
            {
                ExecuteSqlFile(fullPath);
            }


            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static bool CanUseTrim(string fullSqlDataType)
        {
            if (string.IsNullOrWhiteSpace(fullSqlDataType))
                return false;

            // استخراج الاسم الأساسي للنوع (مثلاً: nvarchar من nvarchar(100))
            string baseType = fullSqlDataType
                .Trim()
                .ToLowerInvariant()
                .Split('(')[0];  // مثلاً: "nvarchar(100)" → "nvarchar"

            // قائمة الأنواع النصية التي تقبل LTRIM/RTRIM
            HashSet<string> trimCompatibleTypes = new HashSet<string>
    {
        "char", "nchar", "varchar", "nvarchar", "text", "ntext"
    };

            return trimCompatibleTypes.Contains(baseType);
        }


        private string CreateSP_GetByID()
        {
            string primaryKey = _columns[0]; // Assuming the first column is the primary key
            string primaryKeyType = _dataTypes[0];

            return $@"
CREATE OR ALTER PROCEDURE SP_Get_{_tableName}_ByID
(
    @{primaryKey} {primaryKeyType}
)
AS
BEGIN
    BEGIN TRY
        -- Attempt to retrieve data
        SELECT *
        FROM {_tableName}
        WHERE {primaryKey} = @{primaryKey};
    END TRY
    BEGIN CATCH
        -- Call the centralized error handling procedure
        EXEC SP_HandleError;
    END CATCH
END;
GO";
        }

        private string CreateSP_GetAll()
        {
            return $@"
CREATE OR ALTER PROCEDURE SP_Get_All_{_tableName}
AS
BEGIN
    BEGIN TRY
        -- Attempt to retrieve all data from the table
        SELECT *
        FROM {_tableName};
    END TRY
    BEGIN CATCH
        -- Call the centralized error handling procedure
        EXEC SP_HandleError;
    END CATCH
END;
GO
";
        }

        private string CreateSP_Add()
        {
            StringBuilder parameters = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder columnsList = new StringBuilder();

            for (int i = 1; i < _columns.Length; i++) // Skip primary key for insertion
            {
                string columnName = _columns[i];
                string safeColumnName = $"[{columnName}]"; // وضع الاسم بين [ ]
                string paramName = $"@{columnName.Replace(" ", "")}"; // إزالة الفراغات من اسم المعامل فقط في VALUES

                // إضافة المعاملات (parameters)
                parameters.AppendLine(
                    $"    {paramName} {_dataTypes[i]}{(_nullabilityColumns[i] ? " = NULL" : "")},"
                );

                // إضافة القيم (values)
                string valuePart = CanUseTrim(_dataTypes[i])
                    ? $"LTRIM(RTRIM({paramName}))"
                    : paramName;

                values.AppendLine($"    {valuePart}{(i < _columns.Length - 1 ? "," : "")}");

                // إضافة الأعمدة إلى القائمة
                columnsList.Append($"{safeColumnName}{(i < _columns.Length - 1 ? "," : "")}");
            }

            // إضافة Output Parameter للـ ID الجديد
            parameters.AppendLine($"    @NewID INT OUTPUT");

            string parameterString = parameters.ToString();
            string valuesString = values.ToString().TrimEnd(',', '\n');
            string columnsString = columnsList.ToString().TrimEnd(',', '\n');

            return $@"
CREATE OR ALTER PROCEDURE SP_Add_{_tableName}
(
{parameterString}
)
AS
BEGIN
    BEGIN TRY
        -- Check if any required parameters are NULL
            IF {string.Join(" OR ", _columns.Skip(1).Select((col, i) => !_nullabilityColumns[i + 1] ? (CanUseTrim(_dataTypes[i + 1]) ? $"LTRIM(RTRIM(@{col.Replace(" ", "")})) IS NULL" : $"@{col.Replace(" ", "")} IS NULL") : null).Where(condition => condition != null))}
        BEGIN
            RAISERROR('One or more required parameters are NULL or have only whitespace.', 16, 1);
            RETURN;
        END

        -- Insert the data into the table
        INSERT INTO {_tableName} ({columnsString})
        VALUES ({valuesString});

        -- Set the new ID
        SET @NewID = SCOPE_IDENTITY();  -- Get the last inserted ID
    END TRY
    BEGIN CATCH
        EXEC SP_HandleError; -- Error handling
    END CATCH
END;
GO
";
        }

        private string CreateSP_UpdateByID()
        {
            StringBuilder setClauses = new StringBuilder();
            StringBuilder parameters = new StringBuilder();

            // Iterate through columns to build the SET clause
            for (int i = 1; i < _columns.Length; i++) // Skip the primary key for updates
            {
                string columnName = _columns[i];
                string safeColumnName = $"[{columnName}]"; // Wrap column name in [ ]
                string paramName = $"@{columnName.Replace(" ", "")}"; // Remove spaces from parameter name

                // Add the SET clause with nullability check// Parameters
                parameters.AppendLine(
                    $"    {paramName} {_dataTypes[i]}{(_nullabilityColumns[i] ? " = NULL" : "")}{(i < _columns.Length - 1 ? "," : "")}"
                );

                // Set Clauses
                if (CanUseTrim(_dataTypes[i]))
                {
                    setClauses.AppendLine(
                        $"    {safeColumnName} = LTRIM(RTRIM({paramName})){(i < _columns.Length - 1 ? "," : "")}"
                    );
                }
                else
                {
                    setClauses.AppendLine(
                        $"    {safeColumnName} = {paramName}{(i < _columns.Length - 1 ? "," : "")}"
                    );
                }

            }

            // Add the primary key parameter for the WHERE clause
            string primaryKeyParam = $"@{_columns[0]} {_dataTypes[0]}";

            string setClauseString = setClauses.ToString().TrimEnd(',', '\n');
            string parametersString = parameters.ToString().TrimEnd(',', '\n');

            // Generate the full stored procedure with proper error handling and parameter validation
            return $@"
CREATE OR ALTER PROCEDURE SP_Update_{_tableName}_ByID
(
    {primaryKeyParam},
{parametersString}
)
AS
BEGIN
    BEGIN TRY
        -- Check if required parameters are NULL or contain only whitespace after trimming

        IF {string.Join(" OR ", _columns.Skip(1)
    .Zip(_dataTypes.Skip(1), (col, dt) => new { col, dt })
    .Zip(_nullabilityColumns.Skip(1), (cd, nullable) => new { cd.col, cd.dt, nullable })
    .Where(x => !x.nullable)
    .Select(x => CanUseTrim(x.dt)
        ? $"(LTRIM(RTRIM(@{x.col.Replace(" ", "")})) IS NULL OR LTRIM(RTRIM(@{x.col.Replace(" ", "")})) = '')"
        : $"(@{x.col.Replace(" ", "")} IS NULL OR @{x.col.Replace(" ", "")} = '')"))}

        BEGIN
            RAISERROR('One or more required parameters are NULL or have only whitespace.', 16, 1);
            RETURN;
        END

        -- Update the record in the table
        UPDATE {_tableName}
        SET {setClauseString}
        WHERE {_columns[0]} = @{_columns[0]};
        
        -- Optionally, you can check if the update was successful and raise an error if no rows were updated
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('No rows were updated. Please check the PersonID or other parameters.', 16, 1);
            RETURN;
        END
    END TRY
    BEGIN CATCH
        EXEC SP_HandleError; -- Handle errors
    END CATCH
END;
GO
";
        }

        private string CreateSP_DeleteByID()
        {
            string primaryKey = _columns[0];
            string primaryKeyType = _dataTypes[0];

            return $@"
CREATE OR ALTER PROCEDURE SP_Delete_{_tableName}_ByID
(
    @{primaryKey} {primaryKeyType}
)
AS
BEGIN

    BEGIN TRY
        -- Check if the record exists before attempting to delete
        IF NOT EXISTS (SELECT 1 FROM {_tableName} WHERE {primaryKey} = @{primaryKey})
        BEGIN
            EXEC SP_HandleError;
            RETURN;
        END

        -- Attempt to delete the record
        DELETE FROM {_tableName} WHERE {primaryKey} = @{primaryKey};

        -- Ensure at least one row was deleted
        IF @@ROWCOUNT = 0
        BEGIN
            EXEC SP_HandleError;
            RETURN;
        END
    END TRY
    BEGIN CATCH
        -- Handle all errors (including FK constraint violations)
        EXEC SP_HandleError;
    END CATCH
END;
GO
";
        }

        // First method Search (Search for all data anywhere)

        /*
        private string CreateSP_SearchByColumn()
        {
            // Build the CASE statement for column mapping
            string searchConditions = string.Join("\n        ",
                _columns.Select(col => $"WHEN '{col.Replace(" ", "")}' THEN '{col}'")
            );

            return $@"
CREATE OR ALTER PROCEDURE SP_Search_{_tableName}_ByColumn
(
    @ColumnName NVARCHAR(128),  -- Column name without spaces
    @SearchValue NVARCHAR(255)  -- Value to search for
)
AS
BEGIN
    BEGIN TRY
        DECLARE @ActualColumn NVARCHAR(128);

        -- Map input column name to actual database column name
        SET @ActualColumn = 
            CASE @ColumnName
                {searchConditions}
                ELSE NULL
            END;

        -- Validate the column name
        IF @ActualColumn IS NULL
        BEGIN
            RAISERROR('Invalid Column Name provided.', 16, 1);
            RETURN;
        END

        -- Validate the search value (ensure it's not empty or NULL)
        IF ISNULL(LTRIM(RTRIM(@SearchValue)), '') = ''
        BEGIN
            RAISERROR('Search value cannot be empty.', 16, 1);
            RETURN;
        END

        -- Perform the search query
        DECLARE @SQL NVARCHAR(MAX);
        DECLARE @SearchPattern NVARCHAR(255);

        -- Prepare the search pattern
        SET @SearchPattern = N'%' + LTRIM(RTRIM(@SearchValue)) + N'%';

        -- Build the dynamic SQL query safely
        SET @SQL = N'SELECT * FROM ' + QUOTENAME('{_tableName}') + 
                   N' WHERE ' + QUOTENAME(@ActualColumn) + N' LIKE @SearchPattern OPTION (RECOMPILE)';

        -- Execute the dynamic SQL with parameterized search pattern
        EXEC sp_executesql @SQL, N'@SearchPattern NVARCHAR(255)', @SearchPattern;
    END TRY
    BEGIN CATCH
        -- Handle errors
        EXEC SP_HandleError;
    END CATCH
END;
GO";
        }
        */

        // Second method Search (Search for data in a specific column)

        private string CreateSP_SearchByColumn()
        {
            // Build the CASE statement for column mapping
            string searchConditions = string.Join("\n        ",
                _columns.Select(col => $"WHEN '{col.Replace(" ", "")}' THEN '{col}'")
            );

            return $@"
CREATE OR ALTER PROCEDURE SP_Search_{_tableName}_ByColumn
(
    @ColumnName NVARCHAR(128),  -- Column name without spaces
    @SearchValue NVARCHAR(255), -- Value to search for
    @Mode NVARCHAR(20) = 'Anywhere' -- Search mode (default: Anywhere)
)
AS
BEGIN
    BEGIN TRY
        DECLARE @ActualColumn NVARCHAR(128);
        DECLARE @SQL NVARCHAR(MAX);
        DECLARE @SearchPattern NVARCHAR(255);

        -- Map input column name to actual database column name
        SET @ActualColumn = 
            CASE @ColumnName
                {searchConditions}
                ELSE NULL
            END;

        -- Validate the column name
        IF @ActualColumn IS NULL
        BEGIN
            RAISERROR('Invalid Column Name provided.', 16, 1);
            RETURN;
        END

        -- Validate the search value (ensure it's not empty or NULL)
        IF ISNULL(LTRIM(RTRIM(@SearchValue)), '') = ''
        BEGIN
            RAISERROR('Search value cannot be empty.', 16, 1);
            RETURN;
        END

        -- Prepare the search pattern based on the mode
        SET @SearchPattern =
            CASE 
                WHEN @Mode = 'Anywhere' THEN '%' + LTRIM(RTRIM(@SearchValue)) + '%'
                WHEN @Mode = 'StartsWith' THEN LTRIM(RTRIM(@SearchValue)) + '%'
                WHEN @Mode = 'EndsWith' THEN '%' + LTRIM(RTRIM(@SearchValue))
                WHEN @Mode = 'ExactMatch' THEN LTRIM(RTRIM(@SearchValue))
                ELSE '%' + LTRIM(RTRIM(@SearchValue)) + '%'
            END;

        -- Build the dynamic SQL query safely
        SET @SQL = N'SELECT * FROM ' + QUOTENAME('{_tableName}') + 
                   N' WHERE ' + QUOTENAME(@ActualColumn) + N' LIKE @SearchPattern OPTION (RECOMPILE)';

        -- Execute the dynamic SQL with parameterized search pattern
        EXEC sp_executesql @SQL, N'@SearchPattern NVARCHAR(255)', @SearchPattern;
    END TRY
    BEGIN CATCH
        -- Handle errors
        EXEC SP_HandleError;
    END CATCH
END;
GO";
        }

    }
}
