using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BizDataLayerGen.DataAccessLayer;
using Newtonsoft.Json;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsCreateDataAccessFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        public clsCreateDataAccessFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;
            this._DataTypes = DataTypes;
            this._NullibietyColumns = NullibietyColumns;
        }



        public static string parameterForInsertQueryBuilder(string[] Columns)
        {
            var parameterForInsertQueryBuilder = new StringBuilder();

            for (int i = 1; i < Columns.Length; i++)
            {
                parameterForInsertQueryBuilder.Append($"[{Columns[i]}]");

                if (i < Columns.Length - 1)
                {
                    parameterForInsertQueryBuilder.Append(',');
                }
            }

            return parameterForInsertQueryBuilder.ToString();
        }

        public static string parameterForInsertQueryBuilderValue(string[] Columns)
        {
            var parameterForInsertQueryBuilderValue = new StringBuilder();

            for (int i = 1; i < Columns.Length; i++)
            {
                // إزالة المسافات الداخلية وإضافة @ قبل العمود
                string formattedColumn = "@" + Columns[i].Replace(" ", "");

                parameterForInsertQueryBuilderValue.Append(formattedColumn);

                // إضافة فاصلة فقط إذا لم يكن العنصر الأخير
                if (i < Columns.Length - 1)
                {
                    parameterForInsertQueryBuilderValue.Append(',');
                }
            }

            return parameterForInsertQueryBuilderValue.ToString();
        }

        private string AddDataReaderToVariables()
        {
            var dataReaderCodeBuilder = new StringBuilder();

            for (int i = 1; i < _Columns.Length; i++) // Start from 1 to skip the first column
            {
                string column = _Columns[i].Replace(" ", "");
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                if (isNullable)
                {
                    // Handle nullable columns
                    if (dataType == "string")
                    {
                        dataReaderCodeBuilder.AppendLine(
                            $"                                {column} = reader[\"{_Columns[i]}\"] != DBNull.Value ? reader[\"{_Columns[i]}\"].ToString() : null;");
                    }
                    else
                    {
                        dataReaderCodeBuilder.AppendLine(
                            $"                                {column} = reader[\"{_Columns[i]}\"] != DBNull.Value ? ({dataType}?)reader[\"{_Columns[i]}\"] : null;");
                    }
                }
                else
                {
                    // Handle non-nullable columns
                    dataReaderCodeBuilder.AppendLine(
                        $"                                {column} = ({dataType})reader[\"{_Columns[i]}\"];");
                }
            }

            return dataReaderCodeBuilder.ToString();
        }

        public static string parameterForUpdateQuery(string[] Columns)
        {
            var parameterForInsertQueryBuilder = new StringBuilder();

            for (int i = 1; i < Columns.Length; i++)
            {
                parameterForInsertQueryBuilder.Append($"                                         [{Columns[i]}] = @{Columns[i].Replace(" ", "")}");


                if (i < Columns.Length - 1)
                {
                    parameterForInsertQueryBuilder.AppendLine(",");
                }
            }

            return parameterForInsertQueryBuilder.ToString();
        }


        /*
         SP_Get_TableName_ByID

            SP_Get_All_TableName
            
            SP_Add_TableName
            
            SP_Update_TableName_ByID
            
            SP_Delete_TableName_ByID
            
            SP_Search_TableName_ByColumn
         
         */


        public string AddGetTableInfoByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Get{_TableName}InfoByID({_DataTypes[0]}? {_Columns[0]} {clsGenDataBizLayerMethods.ReferencesCode(_Columns, _DataTypes, _NullibietyColumns)})
{{
    bool isFound = false;

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = ""SP_Get_{_TableName}_ByID"";

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure;

                // Ensure correct parameter assignment
                command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]} ?? (object)DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {{ 
                    if (reader.Read())
                    {{
                        // The record was found
                        isFound = true;

{AddDataReaderToVariables()}
                    }}
                }}
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle all exceptions in a general way
        ErrorHandler.HandleException(ex, nameof(Get{_TableName}InfoByID), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
    }}

    return isFound;
}}";

            return GetTableByIDCode;
        }




        // For Error Handling

        /*
                public string AddGetTableInfoByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Get{_TableName}InfoByID({_DataTypes[0]}? {_Columns[0]} {clsGenDataBizLayerMethods.ReferencesCode(_Columns, _DataTypes, _NullibietyColumns)})
            {{
                bool isFound = false;

                try
                {{
                    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                    {{
                        string query = ""SP_Get_{_TableName}_ByID;"";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {{
                            command.CommandType = CommandType.StoredProcedure;

                            // Add the parameter for ID
                            command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]} ?? (object)DBNull.Value);

                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {{
                                if (reader.Read())
                                {{
                                    // The record was found
                                    isFound = true;

        {AddDataReaderToVariables()}
                                }}
                            }}
                        }}
                    }}
                }}
                catch (SqlException sqlEx)
                {{
                    // Log SQL exception (database-related issue)
                    clsLogger.Log(sqlEx); // افترض وجود دالة Log
                    throw new DataAccessException(""An error occurred while accessing the database."", sqlEx);
                }}
                catch (Exception ex)
                {{
                    // Log general exceptions
                    clsLogger.Log(ex);
                    throw new ApplicationException(""An unexpected error occurred."", ex);
                }}

                return isFound;
            }}";

            return GetTableByIDCode;
        }
        */


        public string AddGetAllDataMethod()
        {
            string GetTableByIDCode = @$"public static DataTable GetAll{_TableName}()
{{
    DataTable dt = new DataTable();

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = ""SP_Get_All_{_TableName}"";

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure; 

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {{
                    if (reader.HasRows)
                    {{
                        dt.Load(reader);
                    }}
                }}
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle all exceptions in a general way
        ErrorHandler.HandleException(ex, nameof(GetAll{_TableName}), ""No parameters for this method."");
    }}

    return dt;
}}";

            return GetTableByIDCode;
        }


        public string AddAddingNewRecordMethod()
        {
            // First Query Is Dynamic Query

            /*
             string query = @""Insert Into {_TableName} ({parameterForInsertQueryBuilder(_Columns)})
                                Values ({parameterForInsertQueryBuilderValue(_Columns)})
                                SELECT SCOPE_IDENTITY();"";

             */


            string GetTableByIDCode = @$"public static int? AddNew{_TableName}({clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns)})
    {{
        int? {_Columns[0]} = null;

        try
        {{
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = @""SP_Add_{_TableName}"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
                    command.CommandType = CommandType.StoredProcedure;

{clsGenDataBizLayerMethods.CreatingCommandParameter(_Columns, _NullibietyColumns)}

                    SqlParameter outputIdParam = new SqlParameter(""@NewID"", SqlDbType.Int)
                    {{
                        Direction = ParameterDirection.Output
                    }};
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    // Bring added value
                    if (outputIdParam.Value != DBNull.Value)
                    {{
                        {_Columns[0]} = (int)outputIdParam.Value;
                    }}

                }}
            }}
        }}
        catch (Exception ex)
        {{
            // Handle all exceptions in a general way
            ErrorHandler.HandleException(ex, nameof(AddNew{_TableName}), $""Parameters: {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns)}"");
        }}

        return {_Columns[0]};
    }}";

            return GetTableByIDCode;
        }

        public string AddUpdatingRecordMethod()
        {

            string GetTableByIDCode = @$"public static bool Update{_TableName}ByID({_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})
{{
    int rowsAffected = 0;

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Update_{_TableName}_ByID""; 

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure;

                // Create the parameters for the stored procedure
{clsGenDataBizLayerMethods.CreatingCommandParameter(_Columns, _NullibietyColumns, 0)}

                // Open the connection and execute the update
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle exceptions
        ErrorHandler.HandleException(ex, nameof(Update{_TableName}ByID), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
    }}

    return (rowsAffected > 0);
}}";



            return GetTableByIDCode;
        }

        public string AddDeleteByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Delete{_TableName}({_DataTypes[0]} {_Columns[0]})
{{
    int rowsAffected = 0;

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Delete_{_TableName}_ByID"";  

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]});

                connection.Open();

                rowsAffected = command.ExecuteNonQuery();
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle all exceptions in a general way, this includes errors from SP_HandleError if any
        ErrorHandler.HandleException(ex, nameof(Delete{_TableName}), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
    }}

    return (rowsAffected > 0);
}}";

            return GetTableByIDCode;
        }


        public string AddSearchMethod()
        {
            string GetTableByIDCode = @$"public static DataTable SearchData(string ColumnName, string SearchValue, string Mode = ""Anywhere"")
{{
    DataTable dt = new DataTable();

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Search_{_TableName}_ByColumn"";

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(""@ColumnName"", ColumnName);
                command.Parameters.AddWithValue(""@SearchValue"", SearchValue);
                command.Parameters.AddWithValue(""@Mode"", Mode);  // Added Mode parameter

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {{
                    if (reader.HasRows)
                    {{
                        dt.Load(reader);
                    }}

                    reader.Close();
                }}
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle all exceptions in a general way
        ErrorHandler.HandleException(ex, nameof(SearchData), $""ColumnName: {{ColumnName}}, SearchValue: {{SearchValue}}, Mode: {{Mode}}"");
    }}

    return dt;
}}";

            return GetTableByIDCode;
        }


        public clsGlobal.enTypeRaisons CreateDataAccessClassFile()
        {



            // First Code 

            /*
             
            string GenerateClassFileForErrorLog = Path.Combine(_filePath, "clsErrorHandlingManager.cs");

            GenerateErrorLogClassesFile(GenerateClassFileForErrorLog);
             
             */




            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");


            

            // Define the code to be written in the file
            string code = $@"
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using {clsGlobal.DataBaseName}_DataAccess;
using Newtonsoft.Json;

namespace {clsGlobal.DataBaseName}_DataLayer
{{
    public class cls{_TableName}Data
    {{
        //#nullable enable

        {AddGetTableInfoByIDMethod()}

        {AddGetAllDataMethod()}

        {AddAddingNewRecordMethod()}

        {AddUpdatingRecordMethod()}

        {AddDeleteByIDMethod()}
        
        {AddSearchMethod()}
    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);
            
            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateDataAccessClassFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            clsCreateDataAccessFile Files = new clsCreateDataAccessFile(filePath, TableName, Columns, DataTypes, NullibietyColumns);

            return Files.CreateDataAccessClassFile();
        }

    }
}
