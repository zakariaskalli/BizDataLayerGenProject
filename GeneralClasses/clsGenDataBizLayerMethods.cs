using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsGenDataBizLayerMethods
    {
        public static string ReferencesCode(string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            var referencesCodeBuilder = new StringBuilder();

            for (int i = 1; i < Columns.Length; i++) // Start from 1 to skip the first column
            {
                string nullableIndicator = NullibietyColumns[i] ? "?" : ""; // Add "?" if the column is nullable
                referencesCodeBuilder.Append($", ref {DataTypes[i]}{nullableIndicator} {Columns[i].Replace(" ", "")}");
            }

            return referencesCodeBuilder.ToString();
        }

        public static string CreatingCommandParameter(string[] Columns, bool[] NullibietyColumns, int StartBy = 1)
        {
            var parameterCommandsBuilder = new StringBuilder();

            for (int i = StartBy; i < Columns.Length; i++) // Start With Second Item
            {
                // Remove Spaces to Add @
                string cleanedColumn = Columns[i].Replace(" ", "");

                // Check if the column is nullable
                if (NullibietyColumns[i])
                {
                    parameterCommandsBuilder.AppendLine(
                        $"                    command.Parameters.AddWithValue(\"@{cleanedColumn}\", {cleanedColumn} ?? (object)DBNull.Value);"
                    );
                }
                else
                {
                    parameterCommandsBuilder.AppendLine(
                        $"                    command.Parameters.AddWithValue(\"@{cleanedColumn}\", {cleanedColumn});"
                    );
                }
            }

            return parameterCommandsBuilder.ToString();
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
            switch (sqlDataType.ToLower())
            {
                case "int": return "int";
                case "bigint": return "long";
                case "smallint": return "short";
                case "tinyint": return "byte";
                case "bit": return "bool";
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney": return "decimal";
                case "float": return "double";
                case "real": return "float";
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext": return "string";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime": return "DateTime";
                case "time": return "TimeSpan";
                case "timestamp":
                case "binary":
                case "varbinary": return "byte[]";
                case "uniqueidentifier": return "Guid";
                default: return "text";
            }
        }


    }
}
