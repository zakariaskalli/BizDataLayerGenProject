using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsGenDataBizLayerMethods
    {
        public static bool CanAcceptNull(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName), "Type name cannot be null or empty.");

            // Map common C# aliases to their fully qualified names
            var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "int", "System.Int32" },
                { "uint", "System.UInt32" },
                { "long", "System.Int64" },
                { "ulong", "System.UInt64" },
                { "short", "System.Int16" },
                { "ushort", "System.UInt16" },
                { "byte", "System.Byte" },
                { "sbyte", "System.SByte" },
                { "char", "System.Char" },
                { "bool", "System.Boolean" },
                { "float", "System.Single" },
                { "double", "System.Double" },
                { "decimal", "System.Decimal" },
                { "string", "System.String" },
                { "object", "System.Object" },
                { "datetime", "System.DateTime" },
                { "timespan", "System.TimeSpan" },
                { "guid", "System.Guid" }
            };

            if (aliases.TryGetValue(typeName, out string systemTypeName))
            {
                typeName = systemTypeName;
            }

            // Try to get the type of the variable from the provided name
            Type type = Type.GetType(typeName);

            if (type == null)
                throw new ArgumentException("Invalid type name provided.");

            // If the type is a reference type (e.g., string or object)
            if (type.IsClass || type == typeof(string) || type.IsInterface)
            {
                return true; // All reference types accept null by default
            }

            // If the type is a value type (e.g., int or bool) and is not nullable
            if (type.IsValueType)
            {
                // If the type is a nullable value type (e.g., int? or bool?)
                return Nullable.GetUnderlyingType(type) != null;
            }

            return false;
        }


        public static string ReferencesCode(string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            var referencesCodeBuilder = new StringBuilder();

            // Ensure that all arrays have the same length
            if (Columns.Length != DataTypes.Length || Columns.Length != NullibietyColumns.Length)
            {
                throw new ArgumentException("The length of Columns, DataTypes, and NullibietyColumns arrays must be the same.");
            }

            // Iterate over columns starting from index 0 (without skipping)
            for (int i = 1; i < Columns.Length; i++)
            {
                // Use CanAcceptNull to check if the type can accept null
                bool canAcceptNull = !(CanAcceptNull(DataTypes[i]));

                // Add the nullable indicator (if the column is nullable and the type accepts null)
                string nullableIndicator = canAcceptNull ? "?" : "";

                //string defaultValue = (NullibietyColumns[i] && !canAcceptNull) ? " = null" : "";

                //referencesCodeBuilder.Append($", ref {DataTypes[i]}{nullableIndicator} {Columns[i].Replace(" ", "").Trim()}{defaultValue}");

                referencesCodeBuilder.Append($", ref {DataTypes[i]}{nullableIndicator} {Columns[i].Replace(" ", "").Trim()}");
            }

            return referencesCodeBuilder.ToString();
        }

        public static string ParameterCode(string[] Columns, string[] DataTypes, bool[] NullibietyColumns, int StartBy = 1)
        {
            var parameterCodeBuilder = new StringBuilder();

            // إنشاء قائمة لتخزين المعاملات غير nullable
            var nonNullableParams = new List<string>();
            // إنشاء قائمة لتخزين المعاملات nullable
            var nullableParams = new List<string>();

            for (int i = StartBy; i < Columns.Length; i++)
            {
                // هل نوع البيانات يمكن أن يكون null؟
                bool canAcceptNull = !(CanAcceptNull(DataTypes[i]));

                // إضافة `?` إذا كان النوع يدعم null
                string nullableIndicator = canAcceptNull ? "?" : "";

                // إذا كان الحقل nullable، نضيف `= null`
                string defaultValue = (NullibietyColumns[i] && !canAcceptNull) ? " = null" : "";

                // تكوين كود المعامل
                string parameter = $"{DataTypes[i]}{nullableIndicator} {Columns[i].Replace(" ", "")}{defaultValue}";

                // إذا لم يكن nullable، أضفه إلى قائمة nonNullableParams
                if (string.IsNullOrEmpty(defaultValue))
                {
                    nonNullableParams.Add(parameter);
                }
                else
                {
                    nullableParams.Add(parameter);
                }
            }

            // ضم المعاملات غير nullable أولًا، ثم nullable
            parameterCodeBuilder.Append(string.Join(", ", nonNullableParams));

            if (nullableParams.Count > 0)
            {
                if (parameterCodeBuilder.Length > 0)
                {
                    parameterCodeBuilder.Append(", ");
                }
                parameterCodeBuilder.Append(string.Join(", ", nullableParams));
            }

            return parameterCodeBuilder.ToString();
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
            // Convert SQL type to lower case for easier matching
            string type = sqlDataType.ToLower();

            switch (type)
            {
                case "int":
                    return "int";
                case "bigint":
                    return "long";
                case "smallint":
                    return "short";
                case "tinyint":
                    return "byte";
                case "bit":
                    return "bool";
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return "decimal";
                case "float":
                    return "double";
                case "real":
                    return "float";
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                    return "string";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "DateTime";
                case "time":
                    return "TimeSpan";
                case "timestamp":
                    return "byte[]"; // In SQL, 'timestamp' is a synonym for 'rowversion'
                case "binary":
                case "varbinary":
                    return "byte[]";
                case "uniqueidentifier":
                    return "Guid";
                case "xml":
                    return "XDocument"; // XML type can be mapped to XDocument or string (for raw XML)
                default:
                    return "object"; // Default fallback for any unknown types
            }
        }


    }
}
