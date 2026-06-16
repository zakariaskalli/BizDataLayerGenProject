using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    { "int?", "null" }, // حماية للـ PK ID والـ FKs
    { "short", "0" },
    { "long", "0" },
    { "float", "0f" },
    { "double", "0.0" },
    { "decimal", "0m" },
    { "string", "string.Empty" },
    { "DateTime", "DateTime.Now" },
    { "DateTime?", "null" }, // حماية لتواريخ النهاية والتواريخ الاختيارية
    { "bool", "false" },
    { "Guid", "Guid.Empty" } // إضافة مفيدة للأنواع الحديثة
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

            sb.AppendLine($"\n       // Flat DTO: contains only the basic fields of the table, used for Add / Update / List operations\r\n");

            // Constructor signature with parameters
            sb.AppendLine($"       public class cls{_TableName}DTO");
            sb.AppendLine("       {");
            sb.AppendLine($"        public {_DataTypes[0]}? {_Columns[0]} {{ get; set; }} = null;");

            // Loop through all the columns starting from index 1
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];


                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Check if the type itself can accept null (for example, reference types or nullable value types)
                bool canAcceptNull = !(clsGenDataBizLayerMethods.CanAcceptNull(dataType));

                string nullableIndicator = (canAcceptNull && isNullable) ? "?" : "";

                //string defaultValue = (isNullable && canAcceptNull) ? " = null;" : "";

                // Otherwise, assign the default value based on the data type.
                string defaultValue = defaultValues.ContainsKey(dataType + nullableIndicator)
                    ? defaultValues[dataType + nullableIndicator]
                    : $"default({dataType})";

                if (dataType == "DateTime" || (dataType + nullableIndicator) == "int")
                {
                    sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }}");
                    continue;
                }

                // Append the property declaration with the default value (if applicable)
                sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }} = {defaultValue};");

            }


            sb.AppendLine("");
            sb.AppendLine($"       public cls{_TableName}DTO() {{}}");
            sb.AppendLine("");

            // Constructor with parameters


            sb.Append($"        public cls{_TableName}DTO(");

            List<string> parametersList = new List<string>();

            // البارامتر الأول الخاص بالـ ID (يستقبل null أو قيمة بشكل طبيعي)
            parametersList.Add($"{_DataTypes[0]}? {_Columns[0]}");

            // Loop لبناء باقي البارامترات كـ Fields عادية بدون أي قيم افتراضية
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                bool canAcceptNull = !(clsGenDataBizLayerMethods.CanAcceptNull(dataType));
                string nullableIndicator = (canAcceptNull && isNullable) ? "?" : "";

                // إضافة النوع والاسم فقط كـ Field عادي
                parametersList.Add($"{dataType}{nullableIndicator} {columnName}");
            }

            // دمج البارامترات بفواصل وإغلاق قوس التوقيع
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


            // Constructor signature with parameters
            sb.AppendLine($"       public class cls{_TableName}DetailsDTO : cls{_TableName}DTO");
            sb.AppendLine("       {");



            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index], referencedColumn = _ReferencedColumn[index] })
                .ToDictionary(x => x.fkColumn, x => new { x.tableName, x.referencedColumn });

            sb.AppendLine("");

            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                string dataType = _DataTypes[i];


                // Check if the column has a foreign key and add the corresponding property
                if (foreignKeyMap.TryGetValue(columnName, out var foreignKey))
                {


                    sb.AppendLine($"        public cls{foreignKey.tableName}DTO {foreignKey.tableName} {{ get; set; }}");
                    

                    continue;
                }

            }

            sb.AppendLine($"         public cls{_TableName}DetailsDTO() : base() {{}}");

            sb.AppendLine("       }");

            return sb.ToString();
        }


        // Inheritance 

        /*
        sb.AppendLine($"        public {_DataTypes[0]}? {_Columns[0]} {{ get; set; }}");


        // Loop through all the columns starting from index 1
        for (int i = 1; i < _Columns.Length; i++)
        {
            string columnName = _Columns[i];


            string dataType = _DataTypes[i];
            bool isNullable = _NullibietyColumns[i];

            // Check if the type itself can accept null (for example, reference types or nullable value types)
            bool canAcceptNull = !(clsGenDataBizLayerMethods.CanAcceptNull(dataType));

            string nullableIndicator = (canAcceptNull && isNullable) ? "?" : "";

            string defaultValue = (isNullable && canAcceptNull) ? " = null;" : "";


            // Append the property declaration with the default value (if applicable)
            sb.AppendLine($"        public {dataType}{nullableIndicator} {columnName} {{ get; set; }}{defaultValue}");



        }
        */

        public clsGlobal.enTypeRaisons CreateDTOLayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}DTO.cs");

            string StringAddFlatDTO = AddAddingFlatDTO(_Columns, _TableName, _DataTypes, _NullibietyColumns);
            string StringAddRichDTO = AddAddingRichDTO(_Columns, _TableName, _DataTypes, _NullibietyColumns);
            

            string code = @$"
using System;
using System.Collections.Generic;

namespace {clsGlobal.DataBaseName}.DTO
{{

{StringAddFlatDTO}

{StringAddRichDTO}

}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

    }
}
