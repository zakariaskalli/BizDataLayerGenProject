using BizDataLayerGen.AI;
using BizDataLayerGen.DataAccessLayer; // Imports layer containing clsGeneralWithData and clsColumnMetadata
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizDataLayerGen.DataAccessLayer.clsGeneralWithData;
using Humanizer;


namespace BizDataLayerGen.GeneralClasses
{
    public class clsCreateDTOFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private string[] _ColumnNamesHasFK;
        private string[] _TablesNameHasFK;
        private string[] _ReferencedColumn;

        Dictionary<string, string> defaultValues = new Dictionary<string, string>
        {
            { "int", "0" },
            { "int?", "null" },
            { "short", "0" },
            { "long", "0" },
            { "float", "0f" },
            { "double", "0.0" },
            { "decimal", "0m" },
            { "string", "string.Empty" },
            { "string?", "null" },
            { "DateTime", "DateTime.Now" },
            { "DateTime?", "null" },
            { "bool", "false" },
            { "Guid", "Guid.Empty" }
        };

        public clsCreateDTOFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                    bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[]
                                    ReferencedColumn)
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
        }

        public string AddAddingFlatDTO(string[] _Columns, string _TableName, string[] _DataTypes, bool[] _NullibietyColumns)
        {
            StringBuilder sb = new StringBuilder();

            // Fetch detailed column metadata to generate Data Annotations / Validation Attributes
            List<clsColumnMetadata> columnsMetadata = clsGeneralWithData.GetTableColumnsMetadata(_TableName, clsGlobal.DataBaseName);

            sb.AppendLine($"\n       // Flat DTO: contains only the basic fields of the table, used for Add / Update / List operations\r\n");

            sb.AppendLine($"       public class cls{_TableName.Singularize()}DTO");
            sb.AppendLine("       {");

            // Primary Key Validation Attribute & Property Definition
            sb.AppendLine($"        [Range(1, int.MaxValue, ErrorMessage = \"Invalid {_Columns[0]}.\")]");
            sb.AppendLine($"        public {_DataTypes[0]}? {_Columns[0]} {{ get; set; }} = null;");

            // Loop through all columns starting from index 1
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Search for metadata corresponding to the current column
                var columnMeta = columnsMetadata?.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                // 1. Generate Validation Attributes
                if (!isNullable)
                {
                    sb.AppendLine($"        [Required(ErrorMessage = \"{columnName} is required.\")]");
                }

                if (columnMeta != null && columnMeta.MaxLengthOrPrecision.HasValue && columnMeta.MaxLengthOrPrecision.Value > 0)
                {
                    if (dataType.Equals("string", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine($"        [StringLength({columnMeta.MaxLengthOrPrecision.Value}, ErrorMessage = \"{columnName} cannot exceed {columnMeta.MaxLengthOrPrecision.Value} characters.\")]");
                    }
                }

                // 2. Generate Class Properties
                string nullableIndicator = isNullable ? "?" : "";

                string defaultValue = defaultValues.ContainsKey(dataType + nullableIndicator)
                    ? defaultValues[dataType + nullableIndicator]
                    : $"default({dataType})";

                if (dataType == "DateTime" || (dataType + nullableIndicator) == "int")
                {
                    sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }}");
                    continue;
                }

                sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }} = {defaultValue};");
            }

            sb.AppendLine("");
            sb.AppendLine($"       public cls{_TableName.Singularize()}DTO() {{}}");
            sb.AppendLine("");

            // Parameterized Constructor
            sb.Append($"        public cls{_TableName.Singularize()}DTO(");

            List<string> parametersList = new List<string>();

            parametersList.Add($"{_DataTypes[0]}? {_Columns[0]}");

            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                string nullableIndicator = isNullable ? "?" : "";
                parametersList.Add($"{dataType}{nullableIndicator} {columnName}");
            }

            sb.Append(string.Join(", ", parametersList));
            sb.AppendLine(")");
            sb.AppendLine("        {");

            foreach (var columnName in _Columns)
            {
                sb.AppendLine($"            this.{columnName} = {columnName};");
            }

            sb.AppendLine("        }");

            sb.AppendLine("       }");

            return sb.ToString();
        }

        public string AddAddingRichDTO(string[] _Columns, string _TableName, string[] _DataTypes, bool[] _NullibietyColumns)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\n       // Rich DTO: contains basic fields + relationships (Navigation DTOs), used for display or API responses\r\n");

            sb.AppendLine($"       public class cls{_TableName.Singularize()}DetailsDTO : cls{_TableName.Singularize()}DTO");
            sb.AppendLine("       {");

            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            sb.AppendLine("");

            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];

                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {
                    sb.AppendLine($"        public cls{foreignKey.tableName.Singularize()}DTO? {foreignKey.tableName.Singularize()} {{ get; set; }} = null;");
                    continue;
                }
            }

            sb.AppendLine($"         public cls{_TableName.Singularize()}DetailsDTO() : base() {{}}");

            sb.AppendLine("       }");

            return sb.ToString();
        }

        public async Task<clsGlobal.enTypeRaisons> CreateDTOLayerFile()
        {
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}DTO.cs");

            string StringAddFlatDTO = AddAddingFlatDTO(_Columns, _TableName, _DataTypes, _NullibietyColumns);
            string StringAddRichDTO = AddAddingRichDTO(_Columns, _TableName, _DataTypes, _NullibietyColumns);

            string code = @$"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace {clsGlobal.ProjectName}.DTO{{{StringAddFlatDTO}{StringAddRichDTO}}}
";

            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;
        }
    }
}