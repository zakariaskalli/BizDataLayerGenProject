using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.DataAccessLayer
{
    public class clsGeneralWithData
    {

        public static bool TestDatabaseConnection()
        {
            // Connection string to your database
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                try
                {
                    // فتح الاتصال
                    connection.Open();
                    // إذا تم فتح الاتصال بنجاح، نعيد true
                    return true;
                }
                catch (Exception ex)
                {
                    // يمكنك تسجيل الخطأ هنا إذا لزم الأمر
                    // Console.WriteLine("Connection Error: " + ex.Message);
                    return false; // إذا فشل الاتصال، نعيد false
                }
            }
        }

        public static bool TestDatabaseConnection(string ConnectionString)
        {
            // Connection string to your database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // فتح الاتصال
                    connection.Open();
                    // إذا تم فتح الاتصال بنجاح، نعيد true
                    return true;
                }
                catch (Exception ex)
                {
                    // يمكنك تسجيل الخطأ هنا إذا لزم الأمر
                    // Console.WriteLine("Connection Error: " + ex.Message);
                    return false; // إذا فشل الاتصال، نعيد false
                }
            }
        }
        

        /*
        public static DataTable GetForeignKeysInfo(string tableName)
        {
            // Define the query to check for foreign keys in the specified table
            string query = @"
            SELECT 
                tp.name AS TableName,
                fk.name AS ForeignKeyName,
                rc.name AS ReferencedTableName
            FROM 
                sys.tables AS tp
            LEFT JOIN 
                sys.foreign_keys AS fk ON fk.parent_object_id = tp.object_id
            LEFT JOIN 
                sys.tables AS rc ON fk.referenced_object_id = rc.object_id
            WHERE 
                tp.name = @TableName";

            // Create a DataTable to hold the result
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add the table name as a parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TableName", tableName);

                try
                {
                    // Open the connection
                    connection.Open();

                    // Execute the query and load the result into the DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return dataTable;
        }
        */


        public static bool HasForeignKey(string tableName)
        {
            // Define the query to check if the specified table has any foreign keys
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
                // Add the table name as a parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TableName", tableName);

                try
                {
                    // Open the connection
                    connection.Open();

                    // Execute the query and get the result
                    int foreignKeyCount = (int)command.ExecuteScalar();

                    // Return true if foreign keys exist, otherwise false
                    return foreignKeyCount > 0;
                }
                catch (Exception ex)
                {
                    return false; // Return false in case of an error
                }

                return false;
            }
        }

        public static string[] GetAllDataBasesName()
        {
            // Initialize a list to store the database names
            List<string> databaseNames = new List<string>();

            // Connection string to your database
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            // SQL query to retrieve database names except 'master'
            string query = "SELECT name FROM sys.databases WHERE name != 'master'";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                // Open the connection
                connection.Open();

                // Execute the query and get the SqlDataReader
                SqlDataReader reader = command.ExecuteReader();

                // Check if the reader has rows
                if (reader.HasRows)
                {
                    // Read each row and add the name to the list
                    while (reader.Read())
                    {
                        databaseNames.Add(reader["name"].ToString());
                    }
                }

                // Close the reader
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle any errors
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection
                connection.Close();
            }

            // Convert the list to an array and return it
            return databaseNames.ToArray();
        }


        public static bool AddDataBaseToSQL(string backupFilePath, string databaseName)
        {
            bool IsAdd = false;


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = $@"RESTORE Database @databaseName 
                                FROM DISK = @backupFilePath";

            SqlCommand Command = new SqlCommand(query, connection);

            Command.Parameters.AddWithValue("@databaseName", databaseName);
            Command.Parameters.AddWithValue("@backupFilePath", backupFilePath);



            try
            {
                connection.Open();

                int result = Command.ExecuteNonQuery();


                if (result > 0)
                    IsAdd = true;
                else
                    IsAdd = false;
            }
            finally
            {
                connection.Close();
            }

            return IsAdd;

        }

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
                            // Read each row and add the name to the list
                            while (reader.Read())
                            {
                                databaseNames.Add(reader["TableName"].ToString());
                            }
                        }

                        // Close the reader
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                // Console.WriteLine("Error: " + ex.Message);
            }

            return databaseNames.ToArray();
        }


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
            }

            return columns.ToArray();
        }

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
                            SELECT DATA_TYPE
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
            }

            return DataTypes.ToArray();
        }

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

            // Test Becuase i delete PrincipaleTable(Users) Variable
            
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
