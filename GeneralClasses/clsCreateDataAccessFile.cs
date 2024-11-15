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

        public clsCreateDataAccessFile(string filePath, string TableName, string[] Columns, string[] DataTypes) 
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;
            this._DataTypes = DataTypes;
        }

        
        public static string ReferencesCode(string[] Columns, string[] DataTypes)
        {
            var referencesCodeBuilder = new StringBuilder();

            foreach (var (column, dataType) in Columns.Skip(1).Zip(DataTypes.Skip(1), (col, dt) => (col, dt)))
            {
                referencesCodeBuilder.Append($", ref {dataType} {column}");
            }

            return referencesCodeBuilder.ToString();
        }

        private string AddDataReaderToVariables()
        {
            var dataReaderCodeBuilder = new StringBuilder();

            foreach (var (column, dataType) in _Columns.Skip(1).Zip(_DataTypes.Skip(1), (col, dt) => (col, dt)))
            {
                dataReaderCodeBuilder.AppendLine($"{column} = ({dataType})reader[\"{column}\"];");
            }

            return dataReaderCodeBuilder.ToString();
        }

        public string AddGetTableInfoByIDMethod()
        {
            string GetTableByIDCode = @$"public static bool Get{_TableName}InfoByID({_DataTypes[0]} {_Columns[0]} {clsGenDataBizLayerMethods.ReferencesCode(_Columns,_DataTypes)})
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

namespace {clsGlobal.DataBaseName}_DataAccess
{{
    public class cls{_TableName}Data
    {{
        {AddGetTableInfoByIDMethod()}

    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateDataAccessClassFile(string filePath, string TableName, string[] Columns, string[] DataTypes)
        {
            clsCreateDataAccessFile Files = new clsCreateDataAccessFile(filePath, TableName, Columns, DataTypes);

            return Files.CreateDataAccessClassFile();
        }
    }
}
