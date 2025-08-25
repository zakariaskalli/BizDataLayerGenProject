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
    public class clsCreateDTOBusinessLayerFile
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

        Dictionary<string, string> defaultValues = new Dictionary<string, string>
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

        public clsCreateDTOBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
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

        public string AddAllFields(string[] _Columns, string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"        // DTO object that holds all booking data\r\n");

            sb.AppendLine($"        public cls{_TableName}DTO Data {{ get; set; }}");

            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            // Loop through all the columns starting from index 1
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];


                // Check if the column has a foreign key and add the corresponding property
                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {

                    sb.AppendLine("");

                    sb.AppendLine($"        private Lazy<cls{foreignKey.tableName}> _{foreignKey.tableName}Info;");
                    sb.AppendLine($"        public cls{foreignKey.tableName} {foreignKey.tableName}Info =>  _{foreignKey.tableName}Info.Value;");
                    
                    sb.AppendLine("");

                }

            }

            return sb.ToString();
        }

        // Consturctors

        public string AddNormalConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature
            sb.AppendLine($"        public cls{_TableName}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            Data = new cls{_TableName}DTO");
            sb.AppendLine("        {");

            // For the primary key (first column), always assign null.
            sb.AppendLine($"            {_Columns[0]} = null,");


            // Loop through the remaining columns to assign default values.
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullableAndCanAcceptNull = _NullibietyColumns[i] && clsGenDataBizLayerMethods.CanAcceptNull(dataType);

                string value = isNullableAndCanAcceptNull
                    ? "null"
                    : (defaultValues.ContainsKey(dataType) ? defaultValues[dataType] : $"default({dataType})");

                // Append comma only if it's not the last column
                sb.AppendLine($"            {columnName} = {value}{(i < _Columns.Length - 1 ? "," : "")}");
            }

            sb.AppendLine("        };");

            // The Lazy Load
            sb.AppendLine("\n");

            //var foreignKeyMap = _ColumnNamesHasFK
            //    .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
            //    .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });
            //
            //foreach (var columnName in _Columns)
            //{
            //    // If the column is a foreign key, add the lookup using the reference column.
            //    if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
            //    {
            //        // Replace with the corresponding referenced column
            //        sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<cls{foreignKey.tableName}>(() => null);");
            //    }
            //}
            //sb.AppendLine("");


            sb.AppendLine("            InitLazyLoaders();");


            sb.AppendLine("            Mode = enMode.AddNew;");
            sb.AppendLine("        }");

            return sb.ToString();
        }


        public string AddUpdateConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string[] _ReferencedColumn)
        {
            
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"        private cls{_TableName}(cls{_TableName}DTO dto)");

            //sb.Append($"{_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})");

            sb.AppendLine("        {");
            sb.AppendLine($"            Data = dto ?? new cls{_TableName}DTO();");

            sb.AppendLine("            InitLazyLoaders();");

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("            Mode = enMode.Update;");

            // Closing the constructor
            sb.AppendLine("        }");

            return sb.ToString();

        }

        public string InitLazyLoaders(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature
            sb.AppendLine($"        private void InitLazyLoaders()");
            sb.AppendLine("        {");

            
            // The Lazy Load
            sb.AppendLine("\n");

            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            foreach (var columnName in _Columns)
            {
                // If the column is a foreign key, add the lookup using the reference column.
                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {
                    sb.AppendLine();

                    // Replace with the corresponding referenced column
                    sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<cls{foreignKey.tableName}>(() => Data.{columnName} > 0 ? cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}(Data.{columnName}) : null);");
                }
            }
            sb.AppendLine("");

            sb.AppendLine("        }");

            return sb.ToString();
        }


        // The Methods  

        public string AddAddingNewRow(string[] _Columns, string _TableName, string[] _DataTypes, bool[] _NullibietyColumns)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _AddNew{_TableName}()");
            sb.AppendLine("       {");

            // Start adding the AddNew call
            sb.AppendLine($"        this.{_Columns[0]} = cls{_TableName}Data.AddNew{_TableName}(Data);");

            // Return a condition checking if the object is not null
            sb.AppendLine($"        return (this.{_Columns[0]} != null);");
            sb.AppendLine("       }");

            return sb.ToString();
        }


        public string AddStaticAddingNewRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool AddNew{_TableName}(cls{_TableName}DTO dto)");

            sb.AppendLine("        {");

            // Start adding the AddNew call
            sb.AppendLine($"            return cls{_TableName}Data.AddNew{_TableName}(dto);");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddUpdateRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _Update{_TableName}()");
            sb.AppendLine("       {");

            // Start adding the Update call
            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName}ByID(Data);");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticUpdateRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool Update{_TableName}ByID(cls{_TableName}DTO dto)");

            sb.AppendLine("        {");

            // Start adding the Update call
            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName}ByID(dto);");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddStaticFind(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static cls{_TableName} FindBy{_Columns[0]}({_DataTypes[0]}? {_Columns[0]})");
            sb.AppendLine(@$"
        {{
            if ({_Columns[0]} == null) return null;");

            sb.AppendLine($"            cls{_TableName}DTO = cls{_TableName}Data.Get{_TableName}InfoByID({_Columns[0]});");


            sb.AppendLine($"                        if (dto == null) return null;\n");

            sb.AppendLine($@"               return new cls{_TableName}(dto);

        }}");

            return sb.ToString();
        }

        public string AddGetAllRows(string[] _Columns, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static List<cls{_TableName}DTO> GetAll{_TableName}()");
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
        public static List<cls{_TableName}DTO> SearchData({_TableName}Column ChosenColumn, string SearchValue, SearchMode Mode = SearchMode.Anywhere)
        {{
            if (string.IsNullOrWhiteSpace(SearchValue) || !SqlHelper.IsSafeInput(SearchValue))
                return new List<cls{_TableName}DTO>();

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


        public clsGlobal.enTypeRaisons CreateDTOBusinessLayerFile()
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

{AddAllFields(_Columns, _ColumnNamesHasFK, _TablesNameHasFK, _TableName)}

        // ---------- Constructors ----------
        // Default AddNew
{AddNormalConstructor(_Columns, _DataTypes, _NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK, _TableName)}
        
        // Private constructor for Update (hydrating from DB)
{AddUpdateConstructor(_Columns,_DataTypes, _NullibietyColumns, _TableName, _ColumnNamesHasFK, _TablesNameHasFK, _ReferencedColumn)}

{InitLazyLoaders(_Columns, _DataTypes, _NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK, _TableName)}


{AddAddingNewRow(_Columns, _TableName, _DataTypes, _NullibietyColumns)}

// We leave in That

{StringAddStaticAddingNewRow}

{AddUpdateRow(_Columns, _DataTypes, _NullibietyColumns, _TableName)}

{StringAddStaticUpdateRow}

{StringAddStaticFind}

{AddSaveRow(_TableName)}

{StringAddGetAllRows}

{StringAddDeleteRow}

{StringAddSearchData}

    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateDTOBusinessLayerFile(string filePath, string TableName, string[] Columns,
            string[] DataTypes, bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[] ReferencedColumn, bool AddingStaticMethods)
        {
            clsCreateDTOBusinessLayerFile Files = new clsCreateDTOBusinessLayerFile(filePath, TableName, Columns, DataTypes,
                                                                        NullibietyColumns, ColumnNamesHasFK, TablesNameHasFK,
                                                                        ReferencedColumn,AddingStaticMethods);

            return Files.CreateDTOBusinessLayerFile();
        }


    }
}
