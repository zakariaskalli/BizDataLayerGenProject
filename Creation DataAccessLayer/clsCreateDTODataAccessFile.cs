using BizDataLayerGen.AI;
using BizDataLayerGen.DataAccessLayer;
using Newtonsoft.Json;
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
using Humanizer;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsCreateDTODataAccessFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private clsGlobal.enExuctionMethods _ExuctionMethod;
        public clsCreateDTODataAccessFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns, clsGlobal.enExuctionMethods ExuctionMethod)
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;
            this._DataTypes = DataTypes;
            this._NullibietyColumns = NullibietyColumns;
            this._ExuctionMethod = ExuctionMethod;
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

            // 1. معالجة مشكلة الـ Normalize للأحجام مثل nvarchar(100) أو decimal(18,2)
            string lowerType = dataType.ToLower().Split('(')[0].Trim();

            // القاموس الموحد لربط الأنواع
            var typeMapping = new Dictionary<string, string>
    {
        { "int", "GetInt32" },
        { "int32", "GetInt32" },
        { "bigint", "GetInt64" },
        { "int64", "GetInt64" },
        { "smallint", "GetInt16" },
        { "int16", "GetInt16" },
        { "tinyint", "GetByte" },
        { "byte", "GetByte" },
        { "bit", "GetBoolean" },
        { "bool", "GetBoolean" },
        { "boolean", "GetBoolean" },
        { "decimal", "GetDecimal" },
        { "numeric", "GetDecimal" },
        { "money", "GetDecimal" },
        { "smallmoney", "GetDecimal" },
        { "float", "GetDouble" },
        { "double", "GetDouble" },
        { "real", "GetFloat" },
        { "char", "GetString" },
        { "varchar", "GetString" },
        { "text", "GetString" },
        { "nchar", "GetString" },
        { "nvarchar", "GetString" },
        { "ntext", "GetString" },
        { "string", "GetString" },
        { "date", "GetDateTime" },
        { "datetime", "GetDateTime" },
        { "datetime2", "GetDateTime" },
        { "smalldatetime", "GetDateTime" },
        { "time", "GetTimeSpan" },
        { "timestamp", "GetValue" },
        { "binary", "GetValue" },
        { "varbinary", "GetValue" },
        { "uniqueidentifier", "GetGuid" },
        { "guid", "GetGuid" },
        { "xml", "GetValue" }
    };

            string readerMethod = typeMapping.ContainsKey(lowerType) ? typeMapping[lowerType] : "GetValue";

            // بناء التعبير بناءً على Nullability
            if (isNullable)
            {
                switch (lowerType)
                {
                    case "int":
                    case "int32": return $"reader.IsDBNull({ordinal}) ? (int?)null : reader.{readerMethod}({ordinal})";
                    case "bigint":
                    case "int64": return $"reader.IsDBNull({ordinal}) ? (long?)null : reader.{readerMethod}({ordinal})";
                    case "smallint":
                    case "int16": return $"reader.IsDBNull({ordinal}) ? (short?)null : reader.{readerMethod}({ordinal})";
                    case "tinyint":
                    case "byte": return $"reader.IsDBNull({ordinal}) ? (byte?)null : reader.{readerMethod}({ordinal})";
                    case "bit":
                    case "bool":
                    case "boolean": return $"reader.IsDBNull({ordinal}) ? (bool?)null : reader.{readerMethod}({ordinal})";
                    case "decimal":
                    case "numeric":
                    case "money":
                    case "smallmoney": return $"reader.IsDBNull({ordinal}) ? (decimal?)null : reader.{readerMethod}({ordinal})";
                    case "float":
                    case "double": return $"reader.IsDBNull({ordinal}) ? (double?)null : reader.{readerMethod}({ordinal})";
                    case "real": return $"reader.IsDBNull({ordinal}) ? (float?)null : reader.{readerMethod}({ordinal})";
                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "string": return $"reader.IsDBNull({ordinal}) ? null : reader.GetString({ordinal})";
                    case "datetime":
                    case "date":
                    case "datetime2":
                    case "smalldatetime": return $"reader.IsDBNull({ordinal}) ? (DateTime?)null : reader.{readerMethod}({ordinal})";
                    case "time": return $"reader.IsDBNull({ordinal}) ? (TimeSpan?)null : reader.{readerMethod}({ordinal})";
                    case "uniqueidentifier":
                    case "guid": return $"reader.IsDBNull({ordinal}) ? (Guid?)null : reader.{readerMethod}({ordinal})";
                    case "timestamp":
                    case "binary":
                    case "varbinary": return $"reader.IsDBNull({ordinal}) ? null : (byte[])reader.GetValue({ordinal})";
                    case "xml": return $"reader.IsDBNull({ordinal}) ? null : (XDocument)reader.GetValue({ordinal})";

                    // إصلاح مشكلة الـ Default لتعيد القيمة الأصلية الخام دون تحويل نصي مكسور
                    default: return $"reader.IsDBNull({ordinal}) ? null : reader.GetValue({ordinal})";
                }
            }
            else
            {
                // Non-nullable
                switch (lowerType)
                {
                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "string": return $"reader.GetString({ordinal})";

                    // إصلاح خطأ الـ Cast المكسور للـ time ليصبح تعبيراً سليماً ومباشراً
                    case "time": return $"reader.{readerMethod}({ordinal})";

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

            dataReaderCodeBuilder.AppendLine(GetReaderExpression(_Columns[0].Replace(" ", ""), _DataTypes[0], false) + ",");

            for (int i = 1; i < _Columns.Length; i++) // Start from 1 to skip the first column
            {
                string column = _Columns[i].Replace(" ", "");
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                if (i == _Columns.Length - 1)
                {
                    dataReaderCodeBuilder.AppendLine( GetReaderExpression(column, dataType, isNullable) );
                }
                else
                {
                    dataReaderCodeBuilder.AppendLine( GetReaderExpression(column, dataType, isNullable) + "," );
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
            string GetTableByIDCode = @$"public static cls{_TableName.Singularize()}DTO? Get{_TableName.Singularize()}InfoByID({_DataTypes[0]}? {_Columns[0]})
{{
    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = ""SP_Get_{_TableName.Singularize()}_ByID"";

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
                        return new cls{_TableName.Singularize()}DTO
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
        ErrorHandler.HandleException(ex, nameof(Get{_TableName.Singularize()}InfoByID), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
    }}
        return null;

}}";

            return GetTableByIDCode;
        }

        public string AddGetAllDataMethod()
        {
            string GetTableByIDCode = @$"public static List<cls{_TableName.Singularize()}DTO> GetAll{_TableName.Pluralize()}()
{{
    var {_TableName.Pluralize()}List = new List<cls{_TableName.Singularize()}DTO>();

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = ""SP_Get_All_{_TableName.Pluralize()}"";

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure; 

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {{
                    while (reader.Read())
                    {{
                        {_TableName.Pluralize()}List.Add(new cls{_TableName.Singularize()}DTO
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
        ErrorHandler.HandleException(ex, nameof(GetAll{_TableName.Pluralize()}), ""No parameters for this method."");
    
    }}

    return {_TableName.Pluralize()}List;
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


            string GetTableByIDCode = @$"public static int? AddNew{_TableName.Singularize()}(cls{_TableName.Singularize()}DTO {_TableName.Singularize()}DTO)
    {{
        int? {_Columns[0]} = null;

        try
        {{
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = @""SP_Add_{_TableName.Singularize()}"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
                    command.CommandType = CommandType.StoredProcedure;

{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName.Singularize())}

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
                        {_TableName.Singularize()}DTO.{_Columns[0]} = (int)outputIdParam.Value;
                    
                    }}

                }}
            }}
        }}
        catch (Exception ex)
        {{
            // Handle all exceptions in a general way
            ErrorHandler.HandleException(ex, nameof(AddNew{_TableName.Singularize()}), $""Parameters: {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns)}"");
        }}

        return {_Columns[0]};
    }}";

            return GetTableByIDCode;
        }


        public string AddUpdatingRecordMethod()
        {

            string GetTableByIDCode = @$"public static bool Update{_TableName.Singularize()}ByID(cls{_TableName.Singularize()}DTO {_TableName.Singularize()}DTO)
{{
    int rowsAffected = 0;

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Update_{_TableName.Singularize()}_ByID""; 

            using (SqlCommand command = new SqlCommand(query, connection))
            {{
                command.CommandType = CommandType.StoredProcedure;

                // Create the parameters for the stored procedure
{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName.Singularize(), 0)}

                // Open the connection and execute the update
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }}
        }}
    }}
    catch (Exception ex)
    {{
        // Handle exceptions
        ErrorHandler.HandleException(ex, nameof(Update{_TableName.Singularize()}ByID), $""Parameter: {_Columns[0]} = "" + {_TableName.Singularize()}DTO.{_Columns[0]});
    }}

    return (rowsAffected > 0);
}}";



            return GetTableByIDCode;
        }


        public string AddDeleteByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Delete{_TableName.Singularize()}({_DataTypes[0]} {_Columns[0]})
{{
    int rowsAffected = 0;

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Delete_{_TableName.Singularize()}_ByID"";  

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
        ErrorHandler.HandleException(ex, nameof(Delete{_TableName.Singularize()}), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
    }}

    return (rowsAffected > 0);
}}";

            return GetTableByIDCode;
        }


        public string AddSearchMethod()
        {
            string GetTableByIDCode = @$"public static List<cls{_TableName.Singularize()}DTO>? SearchData(string ColumnName, string SearchValue, string Mode = ""Anywhere"")
{{
    var {_TableName.Pluralize()}List = new List<cls{_TableName.Singularize()}DTO>();

    try
    {{
        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
        {{
            string query = $@""SP_Search_{_TableName.Singularize()}_ByColumn"";

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
                        {_TableName.Pluralize()}List.Add(new cls{_TableName.Singularize()}DTO
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
        return null;
    }}

    return {_TableName.Pluralize()}List;
}}";

            return GetTableByIDCode;
        }


        // Asynchronous Methods

        public string AddGetTableInfoByIDAsyncMethod()
        {
            string GetTableByIDCode = @$"
        public static async Task<cls{_TableName.Singularize()}DTO?> Get{_TableName.Singularize()}InfoByIDAsync({_DataTypes[0]}? {_Columns[0]}, CancellationToken cancellationToken = default)
        {{
            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SP_Get_{_TableName.Singularize()}_ByID"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.CommandType = CommandType.StoredProcedure;

                        // Ensure correct parameter assignment
                        command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]} ?? (object)DBNull.Value);

                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {{ 
                            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {{
                                return new cls{_TableName.Singularize()}DTO
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
                ErrorHandler.HandleException(ex, nameof(Get{_TableName.Singularize()}InfoByIDAsync), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
            }}
            return null;
        }}";

            return GetTableByIDCode;
        }

        public string AddGetAllDataAsyncMethod()
        {
            string GetTableByIDCode = @$"
        public static async Task<List<cls{_TableName.Singularize()}DTO>> GetAll{_TableName.Pluralize()}Async(CancellationToken cancellationToken = default)
        {{
            var {_TableName.Pluralize()}List = new List<cls{_TableName.Singularize()}DTO>();

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SP_Get_All_{_TableName.Pluralize()}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.CommandType = CommandType.StoredProcedure; 

                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {{
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {{
                                {_TableName.Pluralize()}List.Add(new cls{_TableName.Singularize()}DTO
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
                ErrorHandler.HandleException(ex, nameof(GetAll{_TableName.Pluralize()}Async), ""No parameters for this method."");
            }}

            return {_TableName.Pluralize()}List;
        }}";

            return GetTableByIDCode;
        }

        public string AddAddingNewRecordAsyncMethod()
        {
            string GetTableByIDCode = @$"
    public static async Task<int?> AddNew{_TableName.Singularize()}Async(cls{_TableName.Singularize()}DTO {_TableName.Singularize()}DTO, CancellationToken cancellationToken = default)
    {{
        int? {_Columns[0]} = null;

        try
        {{
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = @""SP_Add_{_TableName.Singularize()}"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
                    command.CommandType = CommandType.StoredProcedure;

{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName.Singularize())}

                    SqlParameter outputIdParam = new SqlParameter(""@NewID"", SqlDbType.Int)
                    {{
                        Direction = ParameterDirection.Output
                    }};
                    command.Parameters.Add(outputIdParam);

                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    // Retrieve the newly inserted identifier
                    if (outputIdParam.Value != DBNull.Value)
                    {{
                        {_Columns[0]} = (int)outputIdParam.Value;
                        {_TableName.Singularize()}DTO.{_Columns[0]} = (int)outputIdParam.Value;
                    
                    }}

                }}
            }}
        }}
        catch (Exception ex)
        {{
            // Handle all exceptions in a general way
            ErrorHandler.HandleException(ex, nameof(AddNew{_TableName.Singularize()}Async), $""Parameters: {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns)}"");
        }}

        return {_Columns[0]};
    }}";

            return GetTableByIDCode;
        }

        public string AddUpdatingRecordAsyncMethod()
        {
            string GetTableByIDCode = @$"
        public static async Task<bool> Update{_TableName.Singularize()}ByIDAsync(cls{_TableName.Singularize()}DTO {_TableName.Singularize()}DTO, CancellationToken cancellationToken = default)
        {{
            int rowsAffected = 0;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = $@""SP_Update_{_TableName.Singularize()}_ByID""; 

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.CommandType = CommandType.StoredProcedure;

                        // Create the parameters for the stored procedure
{clsGenDataBizLayerMethods.CreatingCommandParameterDTO(_Columns, _NullibietyColumns, _TableName.Singularize(), 0)}

                        // Open the connection and execute the update
                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                // Handle exceptions
                ErrorHandler.HandleException(ex, nameof(Update{_TableName.Singularize()}ByIDAsync), $""Parameter: {_Columns[0]} = "" + {_TableName.Singularize()}DTO.{_Columns[0]});
            }}

            return (rowsAffected > 0);
        }}";

            return GetTableByIDCode;
        }

        public string AddDeleteByIDAsyncMethod()
        {
            string GetTableByIDCode = @$"
        public static async Task<bool> Delete{_TableName.Singularize()}Async({_DataTypes[0]} {_Columns[0]}, CancellationToken cancellationToken = default)
        {{
            int rowsAffected = 0;

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = $@""SP_Delete_{_TableName.Singularize()}_ByID"";  

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]});

                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                        rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                // Handle all exceptions in a general way, this includes errors from SP_HandleError if any
                ErrorHandler.HandleException(ex, nameof(Delete{_TableName.Singularize()}Async), $""Parameter: {_Columns[0]} = "" + {_Columns[0]});
            }}

            return (rowsAffected > 0);
        }}";

            return GetTableByIDCode;
        }

        public string AddSearchAsyncMethod()
        {
            string GetTableByIDCode = @$"
        public static async Task<List<cls{_TableName.Singularize()}DTO>?> SearchDataAsync(string ColumnName, string SearchValue, string Mode = ""Anywhere"", CancellationToken cancellationToken = default)
        {{
            var {_TableName.Pluralize()}List = new List<cls{_TableName.Singularize()}DTO>();

            try
            {{
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = $@""SP_Search_{_TableName.Singularize()}_ByColumn"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue(""@ColumnName"", ColumnName);
                        command.Parameters.AddWithValue(""@SearchValue"", SearchValue);
                        command.Parameters.AddWithValue(""@Mode"", Mode);  // Added Mode parameter

                        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {{
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {{
                                {_TableName.Pluralize()}List.Add(new cls{_TableName.Singularize()}DTO
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
                ErrorHandler.HandleException(ex, nameof(SearchDataAsync), $""ColumnName: {{ColumnName}}, SearchValue: {{SearchValue}}, Mode: {{Mode}}"");
                return null;
            }}

            return {_TableName.Pluralize()}List;
        }}";

            return GetTableByIDCode;
        }


        public async Task<clsGlobal.enTypeRaisons> CreateDTODataAccessClassFile()
        {


            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");




            // Define the code to be written in the file
            string code = $@"
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using {clsGlobal.ProjectName}_DataAccess;
using Newtonsoft.Json;
using {clsGlobal.ProjectName}.DTO;

namespace {clsGlobal.ProjectName}_DataLayer
{{
    public class cls{_TableName}Data
    {{
        //#nullable enable

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetTableInfoByIDMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetTableInfoByIDAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetAllDataMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetAllDataAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddAddingNewRecordMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddAddingNewRecordAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddUpdatingRecordMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddUpdatingRecordAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddDeleteByIDMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddDeleteByIDAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddSearchMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddSearchAsyncMethod() : string.Empty)}
    }}
}}
";


            // Write the code to the file
            await Task.Run(() => File.WriteAllText(fullPath, code));
            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static async Task<clsGlobal.enTypeRaisons> CreateDTODataAccessClassFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns, clsGlobal.enExuctionMethods ExuctionMethod)
        {
            clsCreateDTODataAccessFile Files = new clsCreateDTODataAccessFile(filePath, TableName, Columns, DataTypes, NullibietyColumns, ExuctionMethod);

            return await Files.CreateDTODataAccessClassFile();
        }





    }
}
