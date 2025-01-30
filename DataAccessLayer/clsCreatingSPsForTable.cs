using System;
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

        public clsCreatingSPsForTable(string filePath, string tableName, string[] columns, string[] dataTypes, bool[] nullabilityColumns)
        {
            this._filePath = filePath;
            this._tableName = tableName;
            this._columns = columns;
            this._dataTypes = dataTypes;
            this._nullabilityColumns = nullabilityColumns;
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
            sb.AppendLine(CreateSP_GetByID());
            sb.AppendLine(CreateSP_GetAll());
            sb.AppendLine(CreateSP_Add());
            sb.AppendLine(CreateSP_UpdateByID());
            //sb.AppendLine(CreateSP_DeleteByID());
            //sb.AppendLine(CreateSP_SearchByColumn());

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write all SPs to the dynamically generated .sql file
            File.WriteAllText(fullPath, sb.ToString());

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        private string CreateSP_GetByID()
        {
            string primaryKey = _columns[0]; // Assuming the first column is the primary key
            string primaryKeyType = _dataTypes[0];

            return $@"
CREATE PROCEDURE SP_Get_{_tableName}_ByID
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
CREATE PROCEDURE SP_Get_All_{_tableName}
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


        // First Version


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

                // إضافة المعاملات مع التحقق من القيم null
                parameters.AppendLine($"    {paramName} {_dataTypes[i]}{(_nullabilityColumns[i] ? " = NULL" : "")},");

                // إعداد القيم للإدخال مع إضافة التحقق من الفراغات
                values.AppendLine($"    LTRIM(RTRIM({paramName})){(i < _columns.Length - 1 ? "," : "")}");

                // إضافة الأعمدة إلى القائمة
                columnsList.Append($"{safeColumnName}{(i < _columns.Length - 1 ? "," : "")}");
            }

            // إضافة Output Parameter للـ ID الجديد
            parameters.AppendLine($"    @NewID INT OUTPUT");

            string parameterString = parameters.ToString();
            string valuesString = values.ToString().TrimEnd(',', '\n');
            string columnsString = columnsList.ToString().TrimEnd(',', '\n');

            return $@"
CREATE PROCEDURE SP_Add_{_tableName}
(
{parameterString}
)
AS
BEGIN
    BEGIN TRY
        -- Check if any required parameters are NULL
        IF {string.Join(" OR ", _columns.Skip(1).Where((col, i) => !_nullabilityColumns[i + 1]).Select(col => $"LTRIM(RTRIM(@{col.Replace(" ", "")})) IS NULL"))}
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

                // Add the SET clause with nullability check
                setClauses.AppendLine($"    {safeColumnName} = LTRIM(RTRIM({paramName})){(i < _columns.Length - 1 ? "," : "")}");

                // Add parameters with nullability check and trim data for string columns
                parameters.AppendLine($"    {paramName} {_dataTypes[i]}{(_nullabilityColumns[i] ? " = NULL" : "")}{(i < _columns.Length - 1 ? "," : "")}");
            }

            // Add the primary key parameter for the WHERE clause
            string primaryKeyParam = $"@{_columns[0]} {_dataTypes[0]}";

            string setClauseString = setClauses.ToString().TrimEnd(',', '\n');
            string parametersString = parameters.ToString().TrimEnd(',', '\n');

            // Generate the full stored procedure with proper error handling and parameter validation
            return $@"
CREATE PROCEDURE SP_Update_{_tableName}_ByID
(
    {primaryKeyParam},
{parametersString}
)
AS
BEGIN
    BEGIN TRY
        -- Check if required parameters are NULL or contain only whitespace after trimming
        IF {string.Join(" OR ", _columns.Skip(1).Where((col, i) => !_nullabilityColumns[i + 1]).Select(col => $"LTRIM(RTRIM(@{col.Replace(" ", "")})) IS NULL OR LTRIM(RTRIM(@{col.Replace(" ", "")})) = ''"))}
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

CREATE PROCEDURE SP_Delete_{_tableName}_ByID
(
    @{primaryKey} {primaryKeyType}
)
AS
BEGIN
    DELETE FROM {_tableName}
    WHERE {primaryKey} = @{primaryKey};
END;
GO

";
        }

        private string CreateSP_SearchByColumn()
        {
            StringBuilder searchConditions = new StringBuilder();

            // Build search conditions for each column
            for (int i = 0; i < _columns.Length; i++) // Loop through the columns and avoid trailing OR
            {
                searchConditions.AppendLine($"    {_columns[i]} LIKE '%' + @{_columns[i]} + '%'" + (i == _columns.Length - 1 ? "" : " OR"));
            }

            // Build parameters without the trailing comma
            string parameters = string.Join("\n", _columns.Select((col, i) =>
                $"    @{col} {_dataTypes[i]}{(_nullabilityColumns[i] ? " NULL" : " NOT NULL")}"
            ));

            return $@"

CREATE PROCEDURE SP_Search_{_tableName}_ByColumn
(
{parameters}
)
AS
BEGIN
    SELECT *
    FROM {_tableName}
    WHERE {searchConditions.ToString()}
END;
GO

";
        }

    }
}
