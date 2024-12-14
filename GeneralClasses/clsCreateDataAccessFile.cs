using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static string ParameterCode(string[] Columns, string[] DataTypes, bool[] NullibietyColumns, int StartBy = 1)
        {
            var parameterCodeBuilder = new StringBuilder();



            for (int i = StartBy; i < Columns.Length; i++)
            {
                // Add "?" if the column is nullable
                string nullableIndicator = NullibietyColumns[i] ? "?" : "";
                parameterCodeBuilder.Append($"{DataTypes[i]}{nullableIndicator} {Columns[i].Replace(" ", "")}, ");
            }

            // Remove the trailing comma and space
            if (parameterCodeBuilder.Length > 0)
            {
                parameterCodeBuilder.Length -= 2;
            }

            return parameterCodeBuilder.ToString();
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



        public string AddGetTableInfoByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Get{_TableName}InfoByID({_DataTypes[0]}? {_Columns[0]} {clsGenDataBizLayerMethods.ReferencesCode(_Columns, _DataTypes, _NullibietyColumns)})
            {{
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {{
                    string query = ""SELECT * FROM {_TableName} WHERE {_Columns[0]} = @{_Columns[0]}"";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {{
                        command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]});

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
                return isFound;

            }}";

            return GetTableByIDCode;
        }

        public string AddGetAllDataMethod()
        {
            string GetTableByIDCode = @$"public static DataTable GetAll{_TableName}()
{{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {{
        string query = ""SELECT * FROM {_TableName}"";

        using (SqlCommand command = new SqlCommand(query, connection))
        {{

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {{
                if (reader.HasRows)
                    dt.Load(reader);
            }}
        }}
    }}
    return dt;

}}";

            return GetTableByIDCode;
        }

        public string AddAddingNewRecordMethod()
        {


            string GetTableByIDCode = @$" public static int? AddNew{_TableName}({ParameterCode(_Columns, _DataTypes, _NullibietyColumns)})
        {{
            int? {_Columns[0]} = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = @""Insert Into {_TableName} ({parameterForInsertQueryBuilder(_Columns)})
                            Values ({parameterForInsertQueryBuilderValue(_Columns)})
                            SELECT SCOPE_IDENTITY();"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
{clsGenDataBizLayerMethods.CreatingCommandParameter(_Columns, _NullibietyColumns)}

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {{
                        {_Columns[0]} = insertedID;
                    }}
                }}

            }}
            return {_Columns[0]};

        }}
";

            return GetTableByIDCode;
        }

        public string AddUpdatingRecordMethod()
        {


            string GetTableByIDCode = @$" public static bool Update{_TableName}ByID({_DataTypes[0]}? {_Columns[0]}, {ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})
        {{
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                string query = @""Update {_TableName}
                                    set 
{parameterForUpdateQuery(_Columns)}
                                  where [{_Columns[0]}]= @{_Columns[0]}"";

                using (SqlCommand command = new SqlCommand(query, connection))
                {{
{clsGenDataBizLayerMethods.CreatingCommandParameter(_Columns, _NullibietyColumns, 0)}

                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }}

            }}

            return (rowsAffected > 0);
        }}
";

            return GetTableByIDCode;

        }

        public string AddDeleteByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Delete{_TableName}({_DataTypes[0]} {_Columns[0]})
{{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {{
        string query = @""Delete {_TableName} 
                        where {_Columns[0]} = @{_Columns[0]}"";

        using (SqlCommand command = new SqlCommand(query, connection))
        {{
            command.Parameters.AddWithValue(""@{_Columns[0]}"", {_Columns[0]});


            connection.Open();
            
            rowsAffected = command.ExecuteNonQuery();


        }}

    }}
    
    return (rowsAffected > 0);

}}";

            return GetTableByIDCode;
        }

        public string AddSearchMethod()
        {
            string GetTableByIDCode = @$"public static DataTable SearchData(string ColumnName, string Data)
{{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {{
        string query = $@""select * from {_TableName}
                    where {{ColumnName}} Like '' + @Data + '%';"";

        using (SqlCommand Command = new SqlCommand(query, connection))
        {{
            Command.Parameters.AddWithValue(""@Data"", Data);


            connection.Open();

            using (SqlDataReader reader = Command.ExecuteReader())
            {{
                if (reader.HasRows)
                {{
                    dt.Load(reader);
                }}

                reader.Close();
            }}
        }}
        
    }}

    return dt;
}}";

            return GetTableByIDCode;
        }


        public clsGlobal.enTypeRaisons CreateDataAccessClassFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");

            // if you use PK in AllTables, but we have tables Don't have it

            /*
            string PKColmnNameInTable = "";
            
            if (!clsGeneralWithData.GetPrimaryKeyColumnNameFromTable(TableName,ref PKColmnNameInTable))
            {
                return clsGlobal.enTypeRaisons.enTableDontHavePK;
            }
            */


            // Define the code to be written in the file
            string code = $@"
using System;
using System.Data.SqlClient;
using System.Data;
using {clsGlobal.DataBaseName}_DataAccess;

namespace {clsGlobal.DataBaseName}_DataLayer
{{
    public class cls{_TableName}Data
    {{
        #nullable enable

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
