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
        private bool _AddPaggination = false;
        public clsCreateDTODataAccessFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns,bool AddPaggination, clsGlobal.enExuctionMethods ExuctionMethod)
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;
            this._DataTypes = DataTypes;
            this._NullibietyColumns = NullibietyColumns;
            this._ExuctionMethod = ExuctionMethod;
            this._AddPaggination = AddPaggination;
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

            // 1. تنظيف نوع البيانات من System. و النطاقات المزدوجة والأحجام مثل nvarchar(100) أو Nullable<T>
            string lowerType = dataType.ToLower()
                                       .Replace("system.", "")
                                       .Replace("nullable<", "")
                                       .Replace(">", "")
                                       .Split('(')[0]
                                       .Trim();

            // 2. معالجة الحالات بناءً على هل الحقل يقبل Null أم لا
            if (isNullable)
            {
                switch (lowerType)
                {
                    case "smallint":
                    case "int16":
                    case "short": return $"reader.IsDBNull({ordinal}) ? (short?)null : reader.GetInt16({ordinal})";

                    case "int":
                    case "int32":
                    case "integer": return $"reader.IsDBNull({ordinal}) ? (int?)null : reader.GetInt32({ordinal})";

                    case "bigint":
                    case "int64":
                    case "long": return $"reader.IsDBNull({ordinal}) ? (long?)null : reader.GetInt64({ordinal})";

                    case "tinyint":
                    case "byte": return $"reader.IsDBNull({ordinal}) ? (byte?)null : reader.GetByte({ordinal})";

                    case "bit":
                    case "bool":
                    case "boolean": return $"reader.IsDBNull({ordinal}) ? (bool?)null : reader.GetBoolean({ordinal})";

                    case "decimal":
                    case "numeric":
                    case "money":
                    case "smallmoney": return $"reader.IsDBNull({ordinal}) ? (decimal?)null : reader.GetDecimal({ordinal})";

                    case "float":
                    case "double": return $"reader.IsDBNull({ordinal}) ? (double?)null : reader.GetDouble({ordinal})";

                    case "real":
                    case "single": return $"reader.IsDBNull({ordinal}) ? (float?)null : reader.GetFloat({ordinal})";

                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "string":
                    case "sysname": return $"reader.IsDBNull({ordinal}) ? null : reader.GetString({ordinal})";

                    case "datetime":
                    case "date":
                    case "datetime2":
                    case "smalldatetime": return $"reader.IsDBNull({ordinal}) ? (DateTime?)null : reader.GetDateTime({ordinal})";

                    case "time":
                    case "timespan": return $"reader.IsDBNull({ordinal}) ? (TimeSpan?)null : reader.GetTimeSpan({ordinal})";

                    case "uniqueidentifier":
                    case "guid": return $"reader.IsDBNull({ordinal}) ? (Guid?)null : reader.GetGuid({ordinal})";

                    case "timestamp":
                    case "binary":
                    case "varbinary":
                    case "image":
                    case "byte[]": return $"reader.IsDBNull({ordinal}) ? null : (byte[])reader.GetValue({ordinal})";

                    case "xml": return $"reader.IsDBNull({ordinal}) ? null : reader.GetString({ordinal})";

                    default: return $"reader.IsDBNull({ordinal}) ? null : reader.GetValue({ordinal})";
                }
            }
            else
            {
                // Non-nullable (لا يقبل Null)
                switch (lowerType)
                {
                    case "smallint":
                    case "int16": return $"reader.GetInt16({ordinal})";
                    case "int":
                    case "int32": return $"reader.GetInt32({ordinal})";
                    case "bigint":
                    case "int64": return $"reader.GetInt64({ordinal})";
                    case "tinyint":
                    case "byte": return $"reader.GetByte({ordinal})";
                    case "bit":
                    case "bool":
                    case "boolean": return $"reader.GetBoolean({ordinal})";
                    case "decimal":
                    case "numeric":
                    case "money":
                    case "smallmoney": return $"reader.GetDecimal({ordinal})";
                    case "float":
                    case "double": return $"reader.GetDouble({ordinal})";
                    case "real": return $"reader.GetFloat({ordinal})";
                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "string": return $"reader.GetString({ordinal})";
                    case "datetime":
                    case "date":
                    case "datetime2":
                    case "smalldatetime": return $"reader.GetDateTime({ordinal})";

                    case "time":
                    case "timespan": return $"reader.GetTimeSpan({ordinal})";

                    case "uniqueidentifier":
                    case "guid": return $"reader.GetGuid({ordinal})";

                    case "timestamp":
                    case "binary":
                    case "varbinary":
                    case "image":
                    case "byte[]": return $"(byte[])reader.GetValue({ordinal})";

                    case "xml": return $"reader.GetString({ordinal})";

                    default: return $"reader.GetValue({ordinal})";
                }
            }
        }

        private string AddDataReaderToVariablesDTO()
        {
            var dataReaderCodeBuilder = new StringBuilder();

            dataReaderCodeBuilder.AppendLine(GetReaderExpression(_Columns[0].Replace(" ", ""), _DataTypes[0], _NullibietyColumns[0]) + ",");

            for (int i = 1; i < _Columns.Length; i++) // Start from 1 to skip the first column
            {
                string column = _Columns[i].Replace(" ", "");
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                if (i == _Columns.Length - 1)
                {
                    dataReaderCodeBuilder.AppendLine(GetReaderExpression(column, dataType, isNullable));
                }
                else
                {
                    dataReaderCodeBuilder.AppendLine(GetReaderExpression(column, dataType, isNullable) + ",");
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



        public string AddGetAllDataPagginedAsyncMethod()
        {
            string GetTableByIDCode = @$"
            public static async Task<PagedResultDTO<cls{_TableName.Singularize()}DTO>> GetAll{_TableName.Pluralize()}PagedAsync(int PageNumber=1,int PageSize=20,CancellationToken cancellationToken = default)
                {{
                    var {_TableName.Singularize()}Result = new PagedResultDTO<cls{_TableName.Singularize()}DTO>();
                    {_TableName.Singularize()}Result.PageNumber = PageNumber;
                    {_TableName.Singularize()}Result.PageSize = PageSize;

                    try
                    {{
                        using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                        {{
                            string query = ""SP_Get_All_{_TableName.Pluralize()}Paggined"";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {{
                                command.CommandType = CommandType.StoredProcedure; 
                                command.Parameters.AddWithValue(""@PageNumber"", PageNumber);
                                command.Parameters.AddWithValue(""@PageSize"", PageSize);
                                command.Parameters.Add(""@TotalCount"", SqlDbType.Int).Direction = ParameterDirection.Output;

                                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                                using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                                {{
                                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                    {{
                                        {_TableName.Singularize()}Result.Items.Add(new cls{_TableName.Singularize()}DTO
                                        (
                                            {AddDataReaderToVariablesDTO()}
                                        ));
                                    }}
                                }}
                            {_TableName.Singularize()}Result.TotalCount = command.Parameters[""@TotalCount""].Value != DBNull.Value
                        ? (int)command.Parameters[""@TotalCount""].Value
                        : 0;
                            }}
                          
                        }}
                    }}
                    catch (Exception ex)
                    {{
                        // Handle all exceptions in a general way
                        ErrorHandler.HandleException(ex, nameof(GetAll{_TableName.Pluralize()}PagedAsync), ""No parameters for this method."");
                    }}

                    return {_TableName.Singularize()}Result;
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

        public string AddSearchPagginedAsyncMethod()
        {
            string GetTableByIDCode = @$"
    public static async Task<PagedResultDTO<cls{_TableName.Singularize()}DTO>> SearchDataPagedAsync(string ColumnName, string SearchValue, string Mode = ""Anywhere"", int PageNumber = 1, int PageSize = 20, CancellationToken cancellationToken = default)
    {{
        var {_TableName.Singularize()}Result = new PagedResultDTO<cls{_TableName.Singularize()}DTO>();
        {_TableName.Singularize()}Result.PageNumber = PageNumber;
        {_TableName.Singularize()}Result.PageSize = PageSize;

        try
        {{
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = ""SP_Search_{_TableName.Singularize()}_ByColumnPaggined"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(""@ColumnName"", ColumnName);
                    command.Parameters.AddWithValue(""@SearchValue"", SearchValue);
                    command.Parameters.AddWithValue(""@Mode"", Mode);
                    command.Parameters.AddWithValue(""@PageNumber"", PageNumber);
                    command.Parameters.AddWithValue(""@PageSize"", PageSize);
                    command.Parameters.Add(""@TotalCount"", SqlDbType.Int).Direction = ParameterDirection.Output;

                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {{
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {{
                            {_TableName.Singularize()}Result.Items.Add(new cls{_TableName.Singularize()}DTO
                            (
                                {AddDataReaderToVariablesDTO()}
                            ));
                        }}
                    }}

                    {_TableName.Singularize()}Result.TotalCount = command.Parameters[""@TotalCount""].Value != DBNull.Value
                        ? (int)command.Parameters[""@TotalCount""].Value
                        : 0;
                }}
            }}
        }}
        catch (Exception ex)
        {{
            // Handle all exceptions in a general way
            ErrorHandler.HandleException(ex, nameof(SearchDataPagedAsync), $""ColumnName: {{ColumnName}}, SearchValue: {{SearchValue}}, Mode: {{Mode}}, PageNumber: {{PageNumber}}, PageSize: {{PageSize}}"");
        }}

        return {_TableName.Singularize()}Result;
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
using {clsGlobal.ProjectName}.DTO;
using {clsGlobal.ProjectName}.DTO.Common;
using {clsGlobal.ProjectName}_DataAccess;
using Newtonsoft.Json;

namespace {clsGlobal.ProjectName}_DataAccess
{{
    public class cls{_TableName}Data
    {{
        //#nullable enable

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetTableInfoByIDMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetTableInfoByIDAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddGetAllDataMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? _AddPaggination ? AddGetAllDataPagginedAsyncMethod() :  AddGetAllDataAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddAddingNewRecordMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddAddingNewRecordAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddUpdatingRecordMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddUpdatingRecordAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddDeleteByIDMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddDeleteByIDAsyncMethod() : string.Empty)}

        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? AddSearchMethod() : string.Empty)}
        {(_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth ? _AddPaggination ? AddSearchPagginedAsyncMethod() :  AddSearchAsyncMethod() : string.Empty)}
    }}
}}
";

            // Write the code to the file
            await Task.Run(() => File.WriteAllText(fullPath, code));
            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static async Task<clsGlobal.enTypeRaisons> CreateDTODataAccessClassFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns,bool AddingPaggination, clsGlobal.enExuctionMethods ExuctionMethod)
        {
            clsCreateDTODataAccessFile Files = new clsCreateDTODataAccessFile(filePath, TableName, Columns, DataTypes, NullibietyColumns, AddingPaggination, ExuctionMethod);

            return await Files.CreateDTODataAccessClassFile();
        }

    }
}
