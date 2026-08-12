    using BizDataLayerGen.AI;
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
using BizDataLayerGen.GeneralClasses;
using Humanizer;
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
        private clsGlobal.enExuctionMethods _ExuctionMethod;

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
                                    ReferencedColumn, bool AddingStaticMethods, clsGlobal.enExuctionMethods ExuctionMethod)
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
            this._ExuctionMethod = ExuctionMethod;
        }

        public string AddAllFields(string[] _Columns, string[] _ColumnNamesHasFK, string[] _TablesNameHasFK, string _TableName)
        {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"        // DTO object that holds all booking data\r\n");

            sb.AppendLine($"        public cls{_TableName.Singularize()}DTO Data {{ get; set; }}");

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
                    sb.AppendLine();

                    if (_ExuctionMethod == clsGlobal.enExuctionMethods.enBoth || _ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous)
                    {
                        // Async Case
                        sb.AppendLine($"        private Lazy<Task<cls{foreignKey.tableName}?>> _{foreignKey.tableName}Info = null!;");
                        sb.AppendLine($"        public Task<cls{foreignKey.tableName}?> {foreignKey.tableName}Info => _{foreignKey.tableName}Info.Value;");
                    }
                    else
                    {
                        // Sync Case
                        sb.AppendLine($"        private Lazy<cls{foreignKey.tableName}?> _{foreignKey.tableName}Info = null!;");
                        sb.AppendLine($"        public cls{foreignKey.tableName}? {foreignKey.tableName}Info => _{foreignKey.tableName}Info.Value;");
                    }

                    sb.AppendLine();
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
            sb.AppendLine($"            Data = new cls{_TableName.Singularize()}DTO {{}};");
            
            //sb.AppendLine("        {");
            //
            //// For the primary key (first column), always assign null.
            //sb.AppendLine($"            {_Columns[0]} = null,");
            //
            //
            //// Loop through the remaining columns to assign default values.
            //for (int i = 1; i < _Columns.Length; i++)
            //{
            //    string columnName = _Columns[i];
            //    string dataType = _DataTypes[i];
            //    bool isNullableAndCanAcceptNull = _NullibietyColumns[i] && clsGenDataBizLayerMethods.CanAcceptNull(dataType);
            //
            //    string value = isNullableAndCanAcceptNull
            //        ? "null"
            //        : (defaultValues.ContainsKey(dataType) ? defaultValues[dataType] : $"default({dataType})");
            //
            //    // Append comma only if it's not the last column
            //    sb.AppendLine($"            {columnName} = {value}{(i < _Columns.Length - 1 ? "," : "")}");
            //}
            //
            //sb.AppendLine("        };");


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
            sb.AppendLine($"        private cls{_TableName}(cls{_TableName.Singularize()}DTO dto)");

            //sb.Append($"{_DataTypes[0]}? {_Columns[0]}, {clsGenDataBizLayerMethods.ParameterCode(_Columns, _DataTypes, _NullibietyColumns, 1)})");

            sb.AppendLine("        {");
            sb.AppendLine($"            Data = dto ?? new cls{_TableName.Singularize()}DTO();");

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

            sb.AppendLine("        private void InitLazyLoaders()");
            sb.AppendLine("        {");

            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new {
                    fkColumn,
                    tableName = _TablesNameHasFK[index],
                    referencedColumn = _ReferencedColumn[index]
                })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            var nullabilityMap = _Columns
                .Select((col, index) => new { col, isNullable = _NullibietyColumns[index] })
                .ToDictionary(x => x.col, x => x.isNullable);

            foreach (var columnName in _Columns)
            {
                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {
                    sb.AppendLine();

                    bool isNullable = nullabilityMap.TryGetValue(columnName, out var nullable) && nullable;

                    if (_ExuctionMethod == clsGlobal.enExuctionMethods.enBoth || _ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous)
                    {
                        // Async Assignment matching Lazy<Task<clsProject?>>
                        if (isNullable)
                        {
                            sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<Task<cls{foreignKey.tableName}?>>(async () => Data.{columnName}.HasValue ? await cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}Async(Data.{columnName}.Value) : null);");
                        }
                        else
                        {
                            sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<Task<cls{foreignKey.tableName}?>>(async () => await cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}Async(Data.{columnName}));");
                        }
                    }
                    else
                    {
                        // Sync Assignment matching Lazy<clsProject?>
                        if (isNullable)
                        {
                            sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<cls{foreignKey.tableName}?>(() => Data.{columnName}.HasValue ? cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}(Data.{columnName}.Value) : null);");
                        }
                        else
                        {
                            sb.AppendLine($"            _{foreignKey.tableName}Info = new Lazy<cls{foreignKey.tableName}?>(() => cls{foreignKey.tableName}.FindBy{foreignKey.referencedColumn}(Data.{columnName}));");
                        }
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("        }");

            return sb.ToString();
        }
        // The Sync Methods
        public string AddAddingNewRow(string[] _Columns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _AddNew{_TableName.Singularize()}()");
            sb.AppendLine("       {");

            // Start adding the AddNew call
            sb.AppendLine($"           Data.{_Columns[0]} = cls{_TableName}Data.AddNew{_TableName.Singularize()}(Data);");
            // Return a condition checking if the object is not null
            sb.AppendLine($"           return (Data.{_Columns[0]} != null);");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticAddingNewRow(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool AddNew{_TableName.Singularize()}(cls{_TableName.Singularize()}DTO dto)");

            sb.AppendLine("        {");

            // Start adding the AddNew call
            sb.AppendLine($"            return cls{_TableName}Data.AddNew{_TableName.Singularize()}(dto) != null;");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddUpdateRow(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private bool _Update{_TableName.Singularize()}()");
            sb.AppendLine("       {");

            // Start adding the Update call
            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName.Singularize()}ByID(Data);");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticUpdateRow(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool Update{_TableName.Singularize()}ByID(cls{_TableName.Singularize()}DTO dto)");

            sb.AppendLine("        {");

            // Start adding the Update call
            sb.AppendLine($"        return cls{_TableName}Data.Update{_TableName.Singularize()}ByID(dto);");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddStaticFind(string[] _Columns, string[] _DataTypes, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            string pkDataType = _DataTypes[0].Trim();
            if (!pkDataType.EndsWith("?") && !pkDataType.Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                pkDataType += "?";
            }

            // Constructor signature with parameters
            sb.AppendLine($"       public static cls{_TableName}? FindBy{_Columns[0]}({pkDataType} {_Columns[0]})");
            sb.AppendLine(@$"
        {{
            if ({_Columns[0]} == null) return null;");

            sb.AppendLine($"            cls{_TableName.Singularize()}DTO? dto = cls{_TableName}Data.Get{_TableName.Singularize()}InfoByID({_Columns[0]});");

            sb.AppendLine($"                        if (dto == null) return null;\n");

            sb.AppendLine($@"               return new cls{_TableName}(dto);

        }}");

            return sb.ToString();
        }

        public string AddGetAllRows(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static List<cls{_TableName.Singularize()}DTO> GetAll{_TableName.Pluralize()}()");
            sb.AppendLine("       {");
            sb.AppendLine("");

            sb.AppendLine($"        return cls{_TableName}Data.GetAll{_TableName.Pluralize()}() ?? new List<cls{_TableName.Singularize()}DTO>();");

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
                    if(_AddNew{_TableName.Singularize()}())
                    {{
                        Mode = enMode.Update;
                         return true;
                    }}
                    else
                    {{
                        return false;
                    }}
                case enMode.Update:
                    return _Update{_TableName.Singularize()}();

                default:
                    return false;
            }}
        }}
");

            return sb.ToString();
        }

        public string AddDeleteRow(string PKColumnName, string DataTypeForPk, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static bool Delete{_TableName.Singularize()}({DataTypeForPk} {PKColumnName})");
            sb.AppendLine("       {");
            sb.AppendLine("");

            sb.AppendLine($"        return cls{_TableName}Data.Delete{_TableName.Singularize()}({PKColumnName});");

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
        public static List<cls{_TableName.Singularize()}DTO> SearchData({_TableName}Column ChosenColumn, string SearchValue, SearchMode Mode = SearchMode.Anywhere)
        {{
            if (string.IsNullOrWhiteSpace(SearchValue))
                return new List<cls{_TableName.Singularize()}DTO>();

            string modeValue = Mode.ToString(); // Get the mode as string for passing to the stored procedure

            return cls{_TableName}Data.SearchData(ChosenColumn.ToString(), SearchValue, modeValue) ?? new List<cls{_TableName.Singularize()}DTO>();
        }}        
");

            return sb.ToString();
        }


        // The Async Methods

        public string AddAddingNewRowAsync(string[] _Columns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private async Task<bool> _AddNew{_TableName.Singularize()}Async(CancellationToken cancellationToken = default)");
            sb.AppendLine("       {");

            // Start adding the AddNew call
            sb.AppendLine($"           Data.{_Columns[0]} = await cls{_TableName}Data.AddNew{_TableName.Singularize()}Async(Data, cancellationToken).ConfigureAwait(false);");

            // Return a condition checking if the object is not null
            sb.AppendLine($"           return (Data.{_Columns[0]} != null);");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticAddingNewRowAsync(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static async Task<bool> AddNew{_TableName.Singularize()}Async(cls{_TableName.Singularize()}DTO dto, CancellationToken cancellationToken = default)");

            sb.AppendLine("        {");

            // Start adding the AddNew call
            sb.AppendLine($"            return (await cls{_TableName}Data.AddNew{_TableName.Singularize()}Async(dto, cancellationToken).ConfigureAwait(false)) != null;");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddUpdateRowAsync(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       private async Task<bool> _Update{_TableName.Singularize()}Async(CancellationToken cancellationToken = default)");
            sb.AppendLine("       {");

            // Start adding the Update call
            sb.AppendLine($"        return await cls{_TableName}Data.Update{_TableName.Singularize()}ByIDAsync(Data, cancellationToken).ConfigureAwait(false);");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddStaticUpdateRowAsync(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static async Task<bool> Update{_TableName.Singularize()}ByIDAsync(cls{_TableName.Singularize()}DTO dto, CancellationToken cancellationToken = default)");

            sb.AppendLine("        {");

            // Start adding the Update call
            sb.AppendLine($"        return await cls{_TableName}Data.Update{_TableName.Singularize()}ByIDAsync(dto, cancellationToken).ConfigureAwait(false);");

            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddStaticFindAsync(string[] _Columns, string[] _DataTypes, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            string pkDataType = _DataTypes[0].Trim();
            if (!pkDataType.EndsWith("?") && !pkDataType.Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                pkDataType += "?";
            }

            // Constructor signature with parameters
            sb.AppendLine($"       public static async Task<cls{_TableName}?> FindBy{_Columns[0]}Async({pkDataType} {_Columns[0]}, CancellationToken cancellationToken = default)");
            sb.AppendLine(@$"
        {{
            if ({_Columns[0]} == null) return null;");

            sb.AppendLine($"            cls{_TableName.Singularize()}DTO? dto = await cls{_TableName}Data.Get{_TableName.Singularize()}InfoByIDAsync({_Columns[0]}, cancellationToken).ConfigureAwait(false);");

            sb.AppendLine($"                        if (dto == null) return null;\n");

            sb.AppendLine($@"               return new cls{_TableName}(dto);

        }}");

            return sb.ToString();
        }

        public string AddGetAllRowsAsync(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static async Task<List<cls{_TableName.Singularize()}DTO>> GetAll{_TableName.Pluralize()}Async(CancellationToken cancellationToken = default)");
            sb.AppendLine("       {");
            sb.AppendLine("");

            sb.AppendLine($"        return await cls{_TableName}Data.GetAll{_TableName.Pluralize()}Async(cancellationToken).ConfigureAwait(false) ?? new List<cls{_TableName.Singularize()}DTO>();");

            sb.AppendLine("");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddSaveRowAsync(string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($@"
        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {{
            switch (Mode)
            {{
                case enMode.AddNew:
                    if (await _AddNew{_TableName.Singularize()}Async(cancellationToken).ConfigureAwait(false))
                    {{
                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}
                case enMode.Update:
                    return await _Update{_TableName.Singularize()}Async(cancellationToken).ConfigureAwait(false);

                default:
                    return false;
            }}
        }}
");

            return sb.ToString();
        }

        public string AddDeleteRowAsync(string PKColumnName, string DataTypeForPk, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"       public static async Task<bool> Delete{_TableName.Singularize()}Async({DataTypeForPk} {PKColumnName}, CancellationToken cancellationToken = default)");
            sb.AppendLine("       {");
            sb.AppendLine("");

            sb.AppendLine($"        return await cls{_TableName}Data.Delete{_TableName.Singularize()}Async({PKColumnName}, cancellationToken).ConfigureAwait(false);");

            sb.AppendLine("");
            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddSearchDataAsync(string[] _Columns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();
            
            if (_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous) 
            {
                // Add Enum for columns
                sb.AppendLine(EnumForColumns(_Columns, _TableName));

                // Add Enum for Search Modes
                sb.AppendLine(EnumForSearchModes());
            }

            // Constructor signature with parameters
            sb.AppendLine($@"
        public static async Task<List<cls{_TableName.Singularize()}DTO>> SearchDataAsync({_TableName}Column ChosenColumn, string SearchValue, SearchMode Mode = SearchMode.Anywhere, CancellationToken cancellationToken = default)
        {{
            if (string.IsNullOrWhiteSpace(SearchValue))
                return new List<cls{_TableName.Singularize()}DTO>();

            string modeValue = Mode.ToString(); // Get the mode as string for passing to the stored procedure

            return await cls{_TableName}Data.SearchDataAsync(ChosenColumn.ToString(), SearchValue, modeValue, cancellationToken).ConfigureAwait(false) ?? new List<cls{_TableName.Singularize()}DTO>();
        }}        
");

            return sb.ToString();
        }



        public async Task<clsGlobal.enTypeRaisons> CreateDTOBusinessLayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");

            bool isSync = (_ExuctionMethod == clsGlobal.enExuctionMethods.enSynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth);
            bool isAsync = (_ExuctionMethod == clsGlobal.enExuctionMethods.enAsynchronous || _ExuctionMethod == clsGlobal.enExuctionMethods.enBoth);

            // Sync Methods String Generation
            string stringAddAddingNewRow = isSync ? AddAddingNewRow(_Columns, _TableName) : "";
            string stringAddStaticAddingNewRow = (isSync && _AddingStaticMethods) ? AddStaticAddingNewRow(_TableName) : "";
            string stringAddUpdateRow = isSync ? AddUpdateRow(_TableName) : "";
            string stringAddStaticUpdateRow = (isSync && _AddingStaticMethods) ? AddStaticUpdateRow(_TableName) : "";
            string stringAddStaticFind = (isSync && _AddingStaticMethods) ? AddStaticFind(_Columns, _DataTypes, _TableName) : "";
            string stringAddSaveRow = isSync ? AddSaveRow(_TableName) : "";
            string stringAddGetAllRows = (isSync && _AddingStaticMethods) ? AddGetAllRows(_TableName) : "";
            string stringAddDeleteRow = (isSync && _AddingStaticMethods) ? AddDeleteRow(_Columns[0], _DataTypes[0], _TableName) : "";
            string stringAddSearchData = (isSync && _AddingStaticMethods) ? AddSearchData(_Columns, _TableName) : "";

            // Async Methods String Generation
            string stringAddAddingNewRowAsync = isAsync ? AddAddingNewRowAsync(_Columns, _TableName) : "";
            string stringAddStaticAddingNewRowAsync = (isAsync && _AddingStaticMethods) ? AddStaticAddingNewRowAsync(_TableName) : "";
            string stringAddUpdateRowAsync = isAsync ? AddUpdateRowAsync(_TableName) : "";
            string stringAddStaticUpdateRowAsync = (isAsync && _AddingStaticMethods) ? AddStaticUpdateRowAsync(_TableName) : "";
            string stringAddStaticFindAsync = (isAsync && _AddingStaticMethods) ? AddStaticFindAsync(_Columns, _DataTypes, _TableName) : "";
            string stringAddSaveRowAsync = isAsync ? AddSaveRowAsync(_TableName) : "";
            string stringAddGetAllRowsAsync = (isAsync && _AddingStaticMethods) ? AddGetAllRowsAsync(_TableName) : "";
            string stringAddDeleteRowAsync = (isAsync && _AddingStaticMethods) ? AddDeleteRowAsync(_Columns[0], _DataTypes[0], _TableName) : "";
            string stringAddSearchDataAsync = (isAsync && _AddingStaticMethods) ? AddSearchDataAsync(_Columns, _TableName) : "";
            string code = @$"
using System;
using System.Data;
using {clsGlobal.ProjectName}_DataLayer;
using {clsGlobal.ProjectName}.DTO;

namespace {clsGlobal.ProjectName}_BusinessLayer{{
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
{AddUpdateConstructor(_Columns, _DataTypes, _NullibietyColumns, _TableName, _ColumnNamesHasFK, _TablesNameHasFK, _ReferencedColumn)}
{InitLazyLoaders(_Columns, _DataTypes, _NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK, _TableName)}

        // ---------- Sync Methods ----------
{stringAddAddingNewRow}
{stringAddStaticAddingNewRow}
{stringAddUpdateRow}
{stringAddStaticUpdateRow}
{stringAddStaticFind}
{stringAddSaveRow}
{stringAddGetAllRows}
{stringAddDeleteRow}
{stringAddSearchData}

        // ---------- Async Methods ----------
{stringAddAddingNewRowAsync}
{stringAddStaticAddingNewRowAsync}
{stringAddUpdateRowAsync}
{stringAddStaticUpdateRowAsync}
{stringAddStaticFindAsync}
{stringAddSaveRowAsync}
{stringAddGetAllRowsAsync}
{stringAddDeleteRowAsync}
{stringAddSearchDataAsync}

    }}
}}
";

            // Write the code to the file
            await Task.Run(() => File.WriteAllText(fullPath, code));

            return clsGlobal.enTypeRaisons.enPerfect;
        }



        public static Task<clsGlobal.enTypeRaisons> CreateDTOBusinessLayerFile(string filePath, string TableName, string[] Columns,
            string[] DataTypes, bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[] ReferencedColumn, bool AddingStaticMethods, clsGlobal.enExuctionMethods ExuctionMethod)
        {
            clsCreateDTOBusinessLayerFile Files = new clsCreateDTOBusinessLayerFile(filePath, TableName, Columns, DataTypes,
                                                                        NullibietyColumns, ColumnNamesHasFK, TablesNameHasFK,
                                                                        ReferencedColumn,AddingStaticMethods, ExuctionMethod);

            return Files.CreateDTOBusinessLayerFile();
        }


    }
}
