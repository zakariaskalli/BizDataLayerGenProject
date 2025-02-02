using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizDataLayerGen.DataAccessLayer
{
    /// <summary>
    /// Provides general data access methods.
    /// </summary>
    public class clsGeneralWithData
    {
        /// <summary>
        /// Tests the database connection using the default connection string.
        /// </summary>
        /// <returns>True if the connection is successful, otherwise false.</returns>
        public static bool TestDatabaseConnection()
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    var stackTrace = new StackTrace();
                    var frame = stackTrace.GetFrame(0);
                    var method = frame.GetMethod();
                    var className = method.DeclaringType.Name;
                    var methodName = method.Name;

                    ErrorHandler.RaiseError(ex, className, methodName);
                    return false;
                }
            }
        }

        /// <summary>
        /// Tests the database connection using a specified connection string.
        /// </summary>
        /// <param name="ConnectionString">The connection string to use.</param>
        /// <returns>True if the connection is successful, otherwise false.</returns>
        public static bool TestDatabaseConnection(string ConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if the specified table has any foreign keys.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <returns>True if the table has foreign keys, otherwise false.</returns>
        public static bool HasForeignKey(string tableName)
        {
            string query = @"
            SELECT
                COUNT(fk.name) 
            FROM 
                sys.tables AS tp
            INNER JOIN 
                sys.foreign_keys AS fk ON fk.parent_object_id = tp.object_id
            WHERE 
                tp.name = @TableName";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName);

                try
                {
                    connection.Open();
                    int foreignKeyCount = (int)command.ExecuteScalar();
                    return foreignKeyCount > 0;
                }
                catch (Exception ex)
                {
                    var stackTrace = new StackTrace();
                    var frame = stackTrace.GetFrame(0);
                    var method = frame.GetMethod();
                    var className = method.DeclaringType.Name;
                    var methodName = method.Name;

                    ErrorHandler.RaiseError(ex, className, methodName);
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves the names of all databases except 'master'.
        /// </summary>
        /// <returns>An array of database names.</returns>
        public static string[] GetAllDataBasesName()
        {
            List<string> databaseNames = new List<string>();
            const string query = "SELECT name FROM sys.databases  WHERE name != 'master'";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            databaseNames.Add(reader["name"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return databaseNames.ToArray();
        }

        /// <summary>
        /// Adds a database to SQL Server from a backup file.
        /// </summary>
        /// <param name="backupFilePath">The path to the backup file.</param>
        /// <param name="databaseName">The name of the database to add.</param>
        /// <returns>True if the database is added successfully, otherwise false.</returns>
        public static bool AddDataBaseToSQL(string backupFilePath, string databaseName)
        {
            bool IsAdd = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = $@"RESTORE Database @databaseName FROM DISK = @backupFilePath";
            SqlCommand Command = new SqlCommand(query, connection);

            Command.Parameters.AddWithValue("@databaseName", databaseName);
            Command.Parameters.AddWithValue("@backupFilePath", backupFilePath);

            try
            {
                connection.Open();
                int result = Command.ExecuteNonQuery();
                IsAdd = result > 0;
            }
            finally
            {
                connection.Close();
            }

            return IsAdd;
        }

        /// <summary>
        /// Retrieves the names of all tables in the specified database.
        /// </summary>
        /// <param name="DBName">The name of the database.</param>
        /// <returns>An array of table names.</returns>
        public static string[] GetAllTablesByDBName(string DBName)
        {
            List<string> databaseNames = new List<string>();

            if (!clsGeneraleThings.IsValidDatabaseName(DBName))
            {
                return null;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    string query = $@"
                            USE [{DBName}]
                                SELECT name AS TableName
                                FROM sys.tables
                                WHERE is_ms_shipped = 0
                                  AND name NOT IN ('sysdiagrams');";

                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                databaseNames.Add(reader["TableName"].ToString());
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return databaseNames.ToArray();
        }

        /// <summary>
        /// Retrieves the column names of the specified table in the specified database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="DBName">The name of the database.</param>
        /// <returns>An array of column names.</returns>
        public static string[] GetColumnsName(string tableName, string DBName)
        {
            List<string> columns = new List<string>();

            if (!clsGeneraleThings.IsValidDatabaseName(DBName))
            {
                return null;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    string query = $@"
                            Use [{DBName}]
                            SELECT COLUMN_NAME
                             FROM INFORMATION_SCHEMA.COLUMNS
                             WHERE TABLE_NAME = @TableName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["COLUMN_NAME"].ToString();
                                columns.Add(columnName);
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return columns.ToArray();
        }

        /// <summary>
        /// Retrieves the data types of the columns in the specified table in the specified database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="DBName">The name of the database.</param>
        /// <returns>An array of data types.</returns>
        public static string[] GetDataTypes(string tableName, string DBName)
        {
            List<string> DataTypes = new List<string>();

            if (!clsGeneraleThings.IsValidDatabaseName(DBName))
            {
                return null;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    string query = $@"
                                        Use [{DBName}]
                                        SELECT COLUMN_NAME, 
                                               DATA_TYPE
                                        FROM INFORMATION_SCHEMA.COLUMNS
                                        WHERE TABLE_NAME = @TableName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["DATA_TYPE"].ToString();
                                DataTypes.Add(columnName);
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return DataTypes.ToArray();
        }


        public static string[] GetDataTypesForCreating(string tableName, string DBName)
        {
            List<string> DataTypes = new List<string>();

            if (!clsGeneraleThings.IsValidDatabaseName(DBName))
            {
                return null;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    string query = $@"
                            Use [{DBName}]
                            SELECT DATA_TYPE + 
       CASE 
           -- Handle character types with length (e.g., varchar, nvarchar, char, nchar)
           WHEN DATA_TYPE IN ('char', 'varchar', 'nchar', 'nvarchar') THEN 
               CASE 
                   WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN '(MAX)'  -- Handle MAX case
                   ELSE '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
               END
           
           -- Handle numeric types (decimal, numeric) with precision and scale
           WHEN DATA_TYPE IN ('decimal', 'numeric') THEN 
               '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'

           -- Handle exact numeric types (int, smallint, bigint, etc.)
           WHEN DATA_TYPE IN ('int', 'smallint', 'bigint', 'tinyint', 'bit', 'float', 'real') THEN 
               ''  -- No additional info needed for these types

           -- Handle date and time types (datetime, date, time, etc.)
           WHEN DATA_TYPE IN ('datetime', 'date', 'time', 'smalldatetime') THEN 
               ''  -- No additional info needed for these types

           -- Handle binary types (binary, varbinary)
           WHEN DATA_TYPE IN ('binary', 'varbinary') THEN 
               CASE
                   WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN '(MAX)'  -- Handle MAX case for binary/varbinary
                   ELSE '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
               END

           -- Handle other types like uniqueidentifier, xml, etc.
           ELSE 
               ''  -- No additional info needed for other types
       END AS FULL_DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @TableName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["FULL_DATA_TYPE"].ToString();
                                DataTypes.Add(columnName);
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return DataTypes.ToArray();
        }

        /// <summary>
        /// Retrieves the primary key column name of the specified table.
        /// </summary>
        /// <param name="TableName">The name of the table.</param>
        /// <param name="PKColumnName">The primary key column name.</param>
        /// <returns>True if the primary key column name is found, otherwise false.</returns>
        public static bool GetPrimaryKeyColumnNameFromTable(string TableName, ref string PKColumnName)
        {
            bool IsFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                            KU.COLUMN_NAME AS PrimaryKeyColumnName
                         FROM 
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
                         JOIN 
                            INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
                         ON 
                            TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
                         WHERE 
                            TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.TABLE_NAME = @TableName
                         ORDER BY 
                            KU.ORDINAL_POSITION;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", TableName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PKColumnName = reader["PrimaryKeyColumnName"].ToString();
                            IsFound = !string.IsNullOrEmpty(PKColumnName);
                        }
                    }
                }
            }

            return IsFound;
        }

        /// <summary>
        /// Retrieves the nullability of the columns in the specified table in the specified database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="DBName">The name of the database.</param>
        /// <returns>An array of booleans indicating the nullability of each column.</returns>
        public static bool[] GetColumnNullabilityFromTable(string tableName, string DBName)
        {
            List<bool> nullabilities = new List<bool>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @$"
            Use [{DBName}]
            SELECT 
                COLUMN_NAME AS ColumnName,
                CASE IS_NULLABLE WHEN 'YES' THEN 1 ELSE 0 END AS IsNullable
            FROM 
                INFORMATION_SCHEMA.COLUMNS
            WHERE 
                TABLE_NAME = @TableName
            ORDER BY 
                ORDINAL_POSITION;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bool isNullable = (int)reader["IsNullable"] == 1;
                            nullabilities.Add(isNullable);
                        }
                    }
                }
            }

            return nullabilities.ToArray();
        }

        /// <summary>
        /// Retrieves the foreign keys of the specified table in the specified database.
        /// </summary>
        /// <param name="TableName">The name of the table.</param>
        /// <param name="Tables">An array of table names to filter by.</param>
        /// <param name="DBName">The name of the database.</param>
        /// <param name="FKOfAll">A flag to determine whether to include all foreign key relationships or specific ones.</param>
        /// <param name="ColumnNames">An array of foreign key column names.</param>
        /// <param name="TablesName">An array of referenced table names.</param>
        /// <param name="ReferencedColumn">An array of referenced column names.</param>
        /// <returns>True if foreign keys are found, otherwise false.</returns>
        public static bool GetForeignKeysByTableName(string TableName, string[] Tables, string DBName, bool FKOfAll, ref string[] ColumnNames, ref string[] TablesName, ref string[] ReferencedColumn)
        {
            List<string> _ColumnNames = new List<string>();
            List<string> _TablesName = new List<string>();
            List<string> _ReferencedColumn = new List<string>();

            StringBuilder SpecificTables = new StringBuilder();

            foreach (string table in Tables)
            {
                SpecificTables.Append(table + ",");
            }

            SpecificTables.Remove(SpecificTables.Length - 1, 1);

            string query = @$"
USE [{DBName}];

DECLARE @TableName NVARCHAR(128) = @@TableName; -- The main table to check foreign keys for
DECLARE @SpecificTables NVARCHAR(MAX) = @@SpecificTables; -- List of specific tables to filter by
DECLARE @FKOfAll BIT = {(FKOfAll ? "1" : "0")}; -- Flag to determine whether to include all FK relationships or specific ones

-- Create a table variable to hold the filtered table list
DECLARE @TableList TABLE (TableName NVARCHAR(128));
IF (@FKOfAll = 0)
BEGIN
    -- Populate @TableList with specific tables from @SpecificTables
    INSERT INTO @TableList (TableName)
    SELECT LTRIM(RTRIM(value)) -- Trim any extra spaces
    FROM STRING_SPLIT(@SpecificTables, ',');
END

-- Query to retrieve the foreign key relationships
SELECT 
    col.name AS FKColumnName,          -- Foreign key column in the parent table (People)
    refTable.name AS ReferencedTable, -- Name of the referenced table
    refCol.name AS ReferencedColumn   -- Name of the column in the referenced table
FROM 
    sys.foreign_key_columns fk
INNER JOIN 
    sys.columns col 
    ON fk.parent_object_id = col.object_id 
    AND fk.parent_column_id = col.column_id
INNER JOIN 
    sys.tables parentTable 
    ON fk.parent_object_id = parentTable.object_id
INNER JOIN 
    sys.tables refTable 
    ON fk.referenced_object_id = refTable.object_id
INNER JOIN 
    sys.columns refCol 
    ON fk.referenced_object_id = refCol.object_id 
    AND fk.referenced_column_id = refCol.column_id
WHERE 
    parentTable.name = @TableName -- Filter by the parent table
    AND (
        @FKOfAll = 1 -- If all FKs should be included
        OR refTable.name IN (SELECT TableName FROM @TableList) -- If filtering by specific referenced tables
    )
ORDER BY 
    refTable.name, -- Sort by referenced table
    col.name; -- Then by foreign key column name";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@@TableName", TableName);
                    cmd.Parameters.AddWithValue("@@SpecificTables", SpecificTables.ToString());

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nameColumn = (string)reader["FKColumnName"];
                            string tableName = (string)reader["ReferencedTable"];
                            string referencedColumn = (string)reader["ReferencedColumn"];

                            _ColumnNames.Add(nameColumn);
                            _TablesName.Add(tableName);
                            _ReferencedColumn.Add(referencedColumn);
                        }
                    }
                }
            }

            if (_ColumnNames == null || _ColumnNames.Count == 0 || _TablesName == null || _TablesName.Count == 0)
                return false;

            ColumnNames = _ColumnNames.ToArray();
            TablesName = _TablesName.ToArray();
            ReferencedColumn = _ReferencedColumn.ToArray();

            return true;
        }
    }
}