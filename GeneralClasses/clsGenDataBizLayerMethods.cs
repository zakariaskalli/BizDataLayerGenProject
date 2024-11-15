using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsGenDataBizLayerMethods
    {
        public static string ReferencesCode(string[] Columns, string[] DataTypes)
        {
            var referencesCodeBuilder = new StringBuilder();

            foreach (var (column, dataType) in Columns.Skip(1).Zip(DataTypes.Skip(1), (col, dt) => (col, dt)))
            {
                referencesCodeBuilder.Append($", ref {dataType} {column}");
            }

            return referencesCodeBuilder.ToString();
        }

        public static string[] ConvertSqlDataTypesToCSharp(string[] sqlDataTypes)
        {
            List<string> cSharpDataTypes = new List<string>();

            foreach (string sqlDataType in sqlDataTypes)
            {
                string cSharpDataType = MapSqlTypeToCSharpType(sqlDataType);
                cSharpDataTypes.Add(cSharpDataType);
            }

            return cSharpDataTypes.ToArray();
        }

        // Helper method to map SQL data types to C# data types
        public static string MapSqlTypeToCSharpType(string sqlDataType)
        {
            return sqlDataType.ToLower() switch
            {
                "int" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "decimal" => "decimal",
                "numeric" => "decimal",
                "float" => "double",
                "real" => "float",
                "money" => "decimal",
                "smallmoney" => "decimal",
                "char" => "string",
                "varchar" => "string",
                "text" => "string",
                "nchar" => "string",
                "nvarchar" => "string",
                "ntext" => "string",
                "date" => "DateTime",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "smalldatetime" => "DateTime",
                "time" => "TimeSpan",
                "timestamp" => "byte[]",
                "binary" => "byte[]",
                "varbinary" => "byte[]",
                "uniqueidentifier" => "Guid"
                // Default for unrecognized types
            };
        }

    }
}
