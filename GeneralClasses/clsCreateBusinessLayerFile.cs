using BizDataLayerGen.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.GeneralClasses
{
    public class clsCreateBusinessLayerFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private string[] _ColumnNamesHasFK;
        private string[] _TablesNameHasFK;
        private string[] _ReferencedColumn;
        private bool _AddingStaticMethods;

        public clsCreateBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                    bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[] 
                                    ReferencedColumn, bool AddingStaticMethods)
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;

            for (int i = 0; i < _Columns.Length; i++)
            {
                _Columns[i] = _Columns[i].Replace(" ", "");
            }

            this._DataTypes = DataTypes;
            this._NullibietyColumns = NullibietyColumns;
            this._ColumnNamesHasFK = ColumnNamesHasFK;
            this._TablesNameHasFK = TablesNameHasFK;
            this._ReferencedColumn = ReferencedColumn;
            this._AddingStaticMethods = AddingStaticMethods;
        }

        public string AddAllFields(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns,
    string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {

            StringBuilder sb = new StringBuilder();

            // For the first column (PK) - in your DataAccessLayer AddNewPerson method it's null
            // Here we assume the primary key is always nullable (you can adjust if needed)
            sb.AppendLine($"        public {_DataTypes[0]}? {_Columns[0]} {{ get; set; }};");

            // Create a dictionary for FK columns and their corresponding table names for faster lookup
            var foreignKeys = _ColumnNamesHasFK
                .Zip(_TablesNameHasFK, (column, table) => new { column, table })
                .ToDictionary(x => x.column, x => x.table);

            // Loop through all the columns starting from index 1
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Check if the type itself can accept null (for example, reference types or nullable value types)
                bool canAcceptNull = !(clsGenDataBizLayerMethods.CanAcceptNull(dataType));

                string nullableIndicator = canAcceptNull ? "?" : "";

                string defaultValue = (isNullable && !canAcceptNull) ? " = null" : "";

                // Append the property declaration with the default value (if applicable)
                sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }}{defaultValue};");

                // Check if the column has a foreign key and add the corresponding property
                if (foreignKeys.TryGetValue(columnName, out string relatedTable))
                {
                    sb.AppendLine($"        public cls{relatedTable}? {relatedTable}Info {{ get; set; }}");
                }

            }

            return sb.ToString();
        }

        // Consturctors

        public string AddNormalConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature
            sb.AppendLine($"        public cls{_TableName}()");
            sb.AppendLine("        {");

            // For the primary key (first column), always assign null.
            sb.AppendLine($"            this.{_Columns[0]} = null;");

            // Dictionary for default values based on data type.
            var defaultValues = new Dictionary<string, string>
    {
        { "int", "0" },
        { "short", "0" },
        { "long", "0" },
        { "float", "0f" },
        { "double", "0.0" },
        { "decimal", "0m" },
        { "string", "\"\"" },
        { "DateTime", "DateTime.Now" },
        { "bool", "false" }
    };

            // Loop through the remaining columns to assign default values.
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Use the TypeChecker to check if the data type itself can accept null.
                bool canAcceptNull = clsGenDataBizLayerMethods.CanAcceptNull(dataType);

                // Combine the _NullibietyColumns flag and the type's capability.
                bool isNullableAndCanAcceptNull = isNullable && canAcceptNull;

                if (isNullableAndCanAcceptNull)
                {
                    // If the column is marked as nullable and its type can accept null,
                    // assign null as the default value.
                    sb.AppendLine($"            this.{columnName} = null;");
                }
                else
                {
                    // Otherwise, assign the default value based on the data type.
                    string defaultValue = defaultValues.ContainsKey(dataType)
                        ? defaultValues[dataType]
                        : $"default({dataType})";
                    sb.AppendLine($"            this.{columnName} = {defaultValue};");
                }
            }

            sb.AppendLine("            Mode = enMode.AddNew;");
            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddUpdateConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string[] _ReferencedColumn)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"        private cls{_TableName}(");

            sb.Append($"{_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})");

            sb.AppendLine("        {");


            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            foreach (var columnName in _Columns)
            {
                sb.AppendLine($"            this.{columnName} = {columnName};");

                // إذا كان العمود مفتاح خارجي، أضف البحث مع استخدام العمود المرجعي
                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {
                    // Replace with the corresponding referenced column
                    sb.AppendLine($"            this.{foreignKey.tableName}Info = cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}({columnName});");
                }
            }

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("            Mode = enMode.Update;");

            // Closing the constructor
            sb.AppendLine("        }");

            return sb.ToString();

        }


        // The Methods  

        public string AddAddingNewRow(string[] _Columns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _AddNew{_TableName}()");
            sb.AppendLine("       {");
        

            sb.AppendLine($"        this.{_Columns[0]} = cls{_TableName}Data.AddNew{_TableName}(");


            // Loop through the columns and generate the assignments to fields
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($"this.{columnName}");

                if (i < _Columns.Length -1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@");

            return (this.{_Columns[0]} != null);

       }}");

            return sb.ToString();
        }

        public string AddStaticAddingNewRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool AddNew{_TableName}(");

            sb.Append($"ref {_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})");

            sb.AppendLine("        {");

            sb.AppendLine($"        {_Columns[0]} = cls{_TableName}Data.AddNew{_TableName}(");


            // Loop through the columns and generate the assignments to fields
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($"{columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@");

            return ({_Columns[0]} != null);

       }}");

            return sb.ToString();
        }

        public string AddUpdateRow(string[] _Columns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _Update{_TableName}()");
            sb.AppendLine("       {");


            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName}ByID(");


            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($"this.{columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@"       );");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticUpdateRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool Update{_TableName}ByID(");


            sb.Append($"{_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})");

            sb.AppendLine("        {");




            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName}ByID(");


            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($"{columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@");

        }}");

            return sb.ToString();
        }

        public string AddStaticFind(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static cls{_TableName}? FindBy{_Columns[0]}({_DataTypes[0]}? {_Columns[0]})");
            sb.AppendLine(@$"
        {{
            if ({_Columns[0]} == null)
            {{
                return null;
            }}");


            var defaultValues = new Dictionary<string, string>
            {
                { "int", "0" },
                { "short", "0" },
                { "long", "0" },
                { "float", "0f" },
                { "double", "0.0" },
                { "decimal", "0m" },
                { "string", "\"\"" },
                { "DateTime", "DateTime.Now" },
                { "bool", "false" }
            };


            // Create Default value for The Variables
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];



                // Use the TypeChecker to check if the data type itself can accept null.
                bool canAcceptNull = clsGenDataBizLayerMethods.CanAcceptNull(dataType);

                // Combine the _NullibietyColumns flag and the type's capability.
                bool isNullableAndCanAcceptNull = isNullable && canAcceptNull;

                if (isNullableAndCanAcceptNull)
                {
                    // If the column is marked as nullable and its type can accept null,
                    // assign null as the default value.
                    sb.AppendLine($"            {dataType} {columnName} = null;");
                }
                else
                {
                    // Otherwise, assign the default value based on the data type.
                    string defaultValue = defaultValues.ContainsKey(dataType)
                        ? defaultValues[dataType]
                        : $"default({dataType})";
                    sb.AppendLine($"            {dataType} {columnName} = {defaultValue};");
                }
            }




            sb.AppendLine($"            bool IsFound = cls{_TableName}Data.Get{_TableName}InfoByID({_Columns[0]},");

            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($" ref {columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@");");
            sb.AppendLine($@"");


            sb.AppendLine($@"           if(IsFound)");
            sb.AppendLine($@"               return new cls{_TableName}(");

            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.Append($" {columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendLine($@");
            else
                return  null;
        }}");

            return sb.ToString();
        }

        public string AddGetAllRows(string[] _Columns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static DataTable? GetAll{_TableName}()");
            sb.AppendLine("       {");
            sb.AppendLine("");


            sb.AppendLine($"        return cls{_TableName}Data.GetAll{_TableName}();");


            sb.AppendLine("");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddSaveRow(string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($@"
        public bool Save()
        {{
            switch (Mode)
            {{
                case enMode.AddNew:
                    if(_AddNew{_TableName}())
                    {{
                        Mode = enMode.Update;
                         return true;
                    }}
                    else
                    {{
                        return false;
                    }}
                case enMode.Update:
                    return _Update{_TableName}();

            }}
        
            return false;
        }}
");



            return sb.ToString();
        }

        public string AddDeleteRow(string PKColumnName, string DataTypeForPk, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool Delete{_TableName}({DataTypeForPk} {PKColumnName})");
            sb.AppendLine("       {");
            sb.AppendLine("");


            sb.AppendLine($"        return cls{_TableName}Data.Delete{_TableName}({PKColumnName});");


            sb.AppendLine("");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string EnumForColumns(string[] _Columns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"        public enum {_TableName}Column");
            sb.AppendLine("         {");

            for (int i = 0; i < _Columns.Length; i++)
            {
                string column = _Columns[i];
                if (i == _Columns.Length - 1)
                    sb.AppendLine($"            {column}");
                else
                    sb.AppendLine($"            {column},");
            }

            sb.AppendLine("         }");

            return sb.ToString();
        }

        public string EnumForSearchModes()
        {
            return @"
        public enum SearchMode
        {
            Anywhere,
            StartsWith,
            EndsWith,
            ExactMatch
        }
    ";
        }

        public string AddSearchData(string[] _Columns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Add Enum for columns
            sb.AppendLine(EnumForColumns(_Columns, _TableName));

            // Add Enum for Search Modes
            sb.AppendLine(EnumForSearchModes());

            // Constructor signature with parameters
            sb.AppendLine($@"
        public static DataTable? SearchData({_TableName}Column ChosenColumn, string SearchValue, SearchMode Mode = SearchMode.Anywhere)
        {{
            if (string.IsNullOrWhiteSpace(SearchValue) || !SqlHelper.IsSafeInput(SearchValue))
                return new DataTable();

            string modeValue = Mode.ToString(); // Get the mode as string for passing to the stored procedure

            return cls{_TableName}Data.SearchData(ChosenColumn.ToString(), SearchValue, modeValue);
        }}        
");

            return sb.ToString();
        }


        public void AddCheckedTheDataIsSafeMethod()
        {

            string code = @$"
using System;
using System.Data;
using {clsGlobal.DataBaseName}_DataLayer;

namespace {clsGlobal.DataBaseName}_BusinessLayer
{{
    public class SqlHelper
    {{
        public static bool IsSafeInput(string data)
        {{
            if (string.IsNullOrWhiteSpace(data))
                return false; // Input is empty or contains only whitespace
        
            // Check for dangerous patterns or characters commonly used in SQL Injection
            string[] dangerousPatterns = new string[]
            {{
                ""--"",         // SQL comment
                "";"",          // Command terminator
                ""'"",          // Single quote
                ""\"""",         // Double quote
                ""/*"", ""*/"",   // Multi-line comment
                ""xp_"",        // Dangerous stored procedures
                ""exec"",       // Execute commands
                ""select"",     // SQL SELECT statements
                ""insert"",     // SQL INSERT statements
                ""update"",     // SQL UPDATE statements
                ""delete"",     // SQL DELETE statements
                ""drop"",       // Drop tables or databases
                ""create"",     // Create tables or databases
                ""alter""       // Alter tables
            }};
        
            // Convert input to lowercase for case-insensitive checks
            string lowerData = data.ToLower();
        
            // Check if any dangerous pattern exists in the input
            foreach (string pattern in dangerousPatterns)
            {{
                if (lowerData.Contains(pattern))
                {{
                    return false; // Input is unsafe
                }}
            }}
        
            // Ensure input contains only allowed characters (e.g., alphanumeric, underscores, spaces)
            string allowedCharacters = ""abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_ "";
            foreach (char c in data)
            {{
                if (!allowedCharacters.Contains(c.ToString()))
                {{
                    return false; // Input contains disallowed characters
                }}
            }}
        
            return true; // Input is safe
        }}
    }}
}}
";

            string fullPath = Path.Combine(_filePath, $"SqlHelper.cs");

            File.WriteAllText(fullPath, code);

        }


        public clsGlobal.enTypeRaisons CreateBusinessLayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");


            // Add File SqlHelper

            AddCheckedTheDataIsSafeMethod();


            string StringAddStaticAddingNewRow = (_AddingStaticMethods) ? AddStaticAddingNewRow(_Columns, _DataTypes, _NullibietyColumns, _TableName) : "";
            string StringAddStaticUpdateRow = (_AddingStaticMethods) ? AddStaticUpdateRow(_Columns, _DataTypes, _NullibietyColumns, _TableName) : "";
            string StringAddStaticFind = (_AddingStaticMethods) ? AddStaticFind(_Columns, _DataTypes, _NullibietyColumns, _TableName) : "";
            string StringAddGetAllRows = (_AddingStaticMethods) ? AddGetAllRows(_Columns,_TableName) : "";
            string StringAddDeleteRow = (_AddingStaticMethods) ? AddDeleteRow(_Columns[0], _DataTypes[0], _TableName) : "";
            string StringAddSearchData = (_AddingStaticMethods) ? AddSearchData(_Columns, _TableName) : "";

            string code = @$"
using System;
using System.Data;
using {clsGlobal.DataBaseName}_DataLayer;

namespace {clsGlobal.DataBaseName}_BusinessLayer
{{
    public class cls{_TableName}
    {{
        //#nullable enable

        public enum enMode {{ AddNew = 0, Update = 1 }};
        public enMode Mode = enMode.AddNew;

{AddAllFields(_Columns, _DataTypes, _NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK)}

{AddNormalConstructor(_Columns, _DataTypes, _NullibietyColumns, _TableName)}

{AddUpdateConstructor(_Columns,_DataTypes, _NullibietyColumns, _TableName, _ColumnNamesHasFK, _TablesNameHasFK, _ReferencedColumn)}

{AddAddingNewRow(_Columns, _TableName)}

{StringAddStaticAddingNewRow}

{AddUpdateRow(_Columns, _TableName)}

{StringAddStaticUpdateRow}

{StringAddStaticFind}

{StringAddGetAllRows}

{AddSaveRow(_TableName)}

{StringAddDeleteRow}

{StringAddSearchData}

    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateBusinessLayerFile(string filePath, string TableName, string[] Columns,
            string[] DataTypes, bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[] ReferencedColumn, bool AddingStaticMethods)
        {
            clsCreateBusinessLayerFile Files = new clsCreateBusinessLayerFile(filePath, TableName, Columns, DataTypes,
                                                                        NullibietyColumns, ColumnNamesHasFK, TablesNameHasFK,
                                                                        ReferencedColumn,AddingStaticMethods);

            return Files.CreateBusinessLayerFile();
        }


    }
}
