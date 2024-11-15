using Microsoft.VisualBasic.ApplicationServices;
using System;
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
                            SELECT TABLE_NAME
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_TYPE = 'BASE TABLE'";
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Read each row and add the name to the list
                            while (reader.Read())
                            {
                                databaseNames.Add(reader["TABLE_NAME"].ToString());
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


    }
}
