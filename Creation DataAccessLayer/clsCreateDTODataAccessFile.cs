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
    public class clsCreateDTODataAccessFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        public clsCreateDTODataAccessFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
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

        // isn't 100% correct, is 90% because Time and XML Doesn't good handling, and We want some Improvement for Switch
        public static string GetReaderExpression(string columnName, string dataType, bool isNullable)
        {
            string ordinal = $"reader.GetOrdinal(\"{columnName}\")";
            string lowerType = dataType.ToLower();

            // Dictionary يربط أنواع SQL ↔ reader methods أو C# types
            var typeMapping = new Dictionary<string, string>
    {
        { "int", "GetInt32" },
        { "bigint", "GetInt64" },
        { "smallint", "GetInt16" },
        { "tinyint", "GetByte" },
        { "bit", "GetBoolean" },
        { "bool", "GetBoolean" },
        { "boolean", "GetBoolean" },
        { "decimal", "GetDecimal" },
        { "numeric", "GetDecimal" },
        { "money", "GetDecimal" },
        { "smallmoney", "GetDecimal" },
        { "float", "GetDouble" },
        { "real", "GetFloat" },
        { "char", "GetString" },
        { "varchar", "GetString" },
        { "text", "GetString" },
        { "nchar", "GetString" },
        { "nvarchar", "GetString" },
        { "ntext", "GetString" },
        { "date", "GetDateTime" },
        { "datetime", "GetDateTime" },
        { "datetime2", "GetDateTime" },
        { "smalldatetime", "GetDateTime" },
        { "time", "GetTimeSpan" },       // يجب تعريف GetTimeSpan أو fallback لـ GetValue
        { "timestamp", "GetValue" },     // byte[]
        { "binary", "GetValue" },        // byte[]
        { "varbinary", "GetValue" },     // byte[]
        { "uniqueidentifier", "GetGuid" },
        { "xml", "GetValue" },           // XDocument أو string
    };

            // لو النوع مش موجود في dictionary → fallback إلى GetValue
            string readerMethod = typeMapping.ContainsKey(lowerType) ? typeMapping[lowerType] : "GetValue";

            // بناء التعبير مع Nullability
            if (isNullable)
            {
                switch (lowerType)
                {
                    case "int": return $"reader.IsDBNull({ordinal}) ? (int?)null : reader.{readerMethod}({ordinal})";
                    case "bigint": return $"reader.IsDBNull({ordinal}) ? (long?)null : reader.{readerMethod}({ordinal})";
                    case "smallint": return $"reader.IsDBNull({ordinal}) ? (short?)null : reader.{readerMethod}({ordinal})";
                    case "tinyint": return $"reader.IsDBNull({ordinal}) ? (byte?)null : reader.{readerMethod}({ordinal})";
                    case "bit":
                    case "bool":
                    case "boolean": return $"reader.IsDBNull({ordinal}) ? (bool?)null : reader.{readerMethod}({ordinal})";
                    case "decimal":
                    case "numeric":
                    case "money":
                    case "smallmoney": return $"reader.IsDBNull({ordinal}) ? (decimal?)null : reader.{readerMethod}({ordinal})";
                    case "float": return $"reader.IsDBNull({ordinal}) ? (double?)null : reader.{readerMethod}({ordinal})";
                    case "real": return $"reader.IsDBNull({ordinal}) ? (float?)null : reader.{readerMethod}({ordinal})";
                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext": return $"reader.IsDBNull({ordinal}) ? null : reader.{readerMethod}({ordinal})";
                    case "datetime":
                    case "date":
                    case "datetime2":
                    case "smalldatetime": return $"reader.IsDBNull({ordinal}) ? (DateTime?)null : reader.{readerMethod}({ordinal})";
                    case "time": return $"reader.IsDBNull({ordinal}) ? (TimeSpan?)null : reader.{readerMethod}({ordinal})";
                    case "uniqueidentifier": return $"reader.IsDBNull({ordinal}) ? (Guid?)null : reader.{readerMethod}({ordinal})";
                    case "timestamp":
                    case "binary":
                    case "varbinary": return $"reader.IsDBNull({ordinal}) ? null : (byte[])reader.GetValue({ordinal})";
                    case "xml": return $"reader.IsDBNull({ordinal}) ? null : (XDocument)reader.GetValue({ordinal})";
                    default: return $"reader.IsDBNull({ordinal}) ? null : reader.GetValue({ordinal})";
                }
            }
            else
            {
                // Non-nullable → مباشرة reader method أو fallback
                switch (lowerType)
                {
                    case "time": return $"({readerMethod}?)reader.{readerMethod}({ordinal})"; // أو reader.GetValue
                    case "timestamp":
                    case "binary":
                    case "varbinary": return $"(byte[])reader.GetValue({ordinal})";
                    case "xml": return $"(XDocument)reader.GetValue({ordinal})";
                    default: return $"reader.{readerMethod}({ordinal})";
                }
            }
        }


        private string AddDataReaderToVariablesDTO()
        {
            var dataReaderCodeBuilder = new StringBuilder();

            for (int i = 1; i < _Columns.Length; i++) // Start from 1 to skip the first column
            {
                string column = _Columns[i].Replace(" ", "");
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                GetReaderExpression(column, dataType, isNullable);

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


        // Sturcture of Methods to Create in Data Access Layer

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
            string GetTableByIDCode = @$"public static cls{_TableName}DTO Get{_TableName}InfoByID({_DataTypes[0]}? {_Columns[0]})
{{
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
                        return new cls{_TableName}DTO
                        (
                            {AddDataReaderToVariablesDTO()}
                        );
                    }}
                    else
                    {{
                        return null;
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

}}";

            return GetTableByIDCode;
        }


        public string AddGetAllDataMethod()
        {
            string GetTableByIDCode = @$"public static List<cls{_TableName}DTO> GetAll{_TableName}()
{{
    var {_TableName}List = new List<cls{_TableName}DTO>();

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
                    while (reader.Read())
                    {{
                        {_TableName}List.Add(new cls{_TableName}DTO
                        (
                            {AddDataReaderToVariablesDTO()}
                        ));
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

    return {_TableName}List;
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


            string GetTableByIDCode = @$"public static int? AddNew{_TableName}( cls{_TableName}DTO {_TableName}DTO)
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

{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName)}

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
{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName, 0)}

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
            string GetTableByIDCode = @$"public static List<cls{_TableName}DTO> SearchData(string ColumnName, string SearchValue, string Mode = ""Anywhere"")
{{
    var {_TableName}List = new List<cls{_TableName}DTO>();

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
                    while (reader.Read())
                    {{
                        {_TableName}List.Add(new cls{_TableName}DTO
                        (
                            {AddDataReaderToVariablesDTO()}
                        ));
                    }}
                }}
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle all exceptions in a general way
        ErrorHandler.HandleException(ex, nameof(SearchData), $""ColumnName: {{ColumnName}}, SearchValue: {{SearchValue}}, Mode: {{Mode}}"");
    }}

    return {_TableName}List;
}}";

            return GetTableByIDCode;
        }


        public clsGlobal.enTypeRaisons CreateDTODataAccessClassFile()
        {


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

        public static clsGlobal.enTypeRaisons CreateDTODataAccessClassFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            clsCreateDTODataAccessFile Files = new clsCreateDTODataAccessFile(filePath, TableName, Columns, DataTypes, NullibietyColumns);

            return Files.CreateDTODataAccessClassFile();
        }

    }
}
