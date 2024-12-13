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
        private bool _AddingStaticMethods;

        public clsCreateBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                    bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK
            , bool AddingStaticMethods)
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
            this._AddingStaticMethods = AddingStaticMethods;
        }

        public string AddAllFields(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {
            
            StringBuilder sb = new StringBuilder();
            // This For the First Column in Any Function (PK) Because He is null in AddNewPerson Methode In DataAccessLayer
            sb.AppendLine($"        public {_DataTypes[0]}? {_Columns[0]} {{ get; set; }}");


            // The old Algorithm

            /*
            
            // Loop through all the columns and create the properties dynamically
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Generate property declaration
                sb.AppendLine($"        public {dataType}{(isNullable ? "?" : "")} {columnName} {{ get; set; }}");

                if (_ColumnNamesHasFK.Length > 0 && _TablesNameHasFK.Length > 0)
                {
                    // You can to more diminue this loop 
                    for (int j = 0; j < _ColumnNamesHasFK.Length; j++)
                    {
                        // edit columnName is not string is variable
                        if (_ColumnNamesHasFK[j] == columnName)
                        {
                            sb.AppendLine($"        public cls{_TablesNameHasFK[j]} {_TablesNameHasFK[j]}Info;");
                        }
                    }
                }
            
            
            }
            */


                        // The New Algotithm More Optimze Use Dicationary 


            // Create a dictionary for FK columns and their corresponding table names for faster lookup
            var foreignKeys = _ColumnNamesHasFK
                .Zip(_TablesNameHasFK, (column, table) => new { column, table })
                .ToDictionary(x => x.column, x => x.table);

            // Loop through all the columns to create properties
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Add the property for the column
                sb.AppendLine($"        public {dataType}{(isNullable ? "?" : "")} {columnName} {{ get; set; }}");

                // Check if the column has a foreign key and add the corresponding property
                if (foreignKeys.TryGetValue(columnName, out string relatedTable))
                {
                    sb.AppendLine($"        public cls{relatedTable} {relatedTable}Info {{ get; set; }}");
                }
            }

            return sb.ToString();

            
        }

        public string AddNormalConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            StringBuilder sb = new StringBuilder();

            // Constructor signature
            sb.AppendLine($"        public cls{_TableName}()");

            // Constructor body
            sb.AppendLine("        {");

            sb.AppendLine($"            this.{_Columns[0]} = null;");

            // The old Algorithm

            /*
            // Loop through all the columns and generate the assignments
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Check if the column is nullable and assign default value
                if (isNullable)
                {
                    sb.AppendLine($"    this.{columnName} = null;");
                }
                else
                {
                    // Assign default values based on data type
                    if (dataType == "int" || dataType == "short" || dataType == "long" || dataType == "float" || dataType == "double" || dataType == "decimal")
                    {
                        sb.AppendLine($"    this.{columnName} = 0;");
                    }
                    else if (dataType == "string")
                    {
                        sb.AppendLine($"    this.{columnName} = \"\";");
                    }
                    else if (dataType == "DateTime")
                    {
                        sb.AppendLine($"    this.{columnName} = DateTime.Now;");
                    }
                    else if (dataType == "bool")
                    {
                        sb.AppendLine($"    this.{columnName} = false;");
                    }
                    else
                    {
                        sb.AppendLine($"    this.{columnName} = default({dataType});");
                    }
                }
            }
            */

                        // The New Algotithm More Optimze Use Dicationary 

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

            // تعيين قيمة العمود الأول (Primary Key) إلى null دائمًا
            sb.AppendLine($"            this.{_Columns[0]} = null;");

            // إنشاء القيم الافتراضية لباقي الأعمدة
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // تحديد القيمة الافتراضية باستخدام القاموس
                if (isNullable)
                {
                    sb.AppendLine($"            this.{columnName} = null;");
                }
                else
                {
                    string defaultValue = defaultValues.ContainsKey(dataType)
                        ? defaultValues[dataType]
                        : $"default({dataType})";
                    sb.AppendLine($"            this.{columnName} = {defaultValue};");
                }
            }

            // Closing the constructor
            sb.AppendLine("            Mode = enMode.AddNew;");
            sb.AppendLine("        }");

            return sb.ToString();
        }

        public string AddUpdateConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {

            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"        private cls{_TableName}(");

            sb.Append($"{_DataTypes[0]}? {_Columns[0]},");

            // Loop through columns and generate constructor parameters
            for (int i = 1; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];
                bool isNullable = _NullibietyColumns[i];

                // Add parameter to the constructor
                sb.Append($"{dataType}{(isNullable ? "?" : "")} {columnName}");

                if (i < _Columns.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            // Close the parameter list and open the constructor body
            sb.AppendLine("          )");

            sb.AppendLine("        {");

            // The old Algorithm

            /*
             
            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];

                sb.AppendLine($"    this.{columnName} = {columnName};");


                if (_ColumnNamesHasFK.Length > 0 && _TablesNameHasFK.Length > 0)
                {
                    for (int j = 0; j < _ColumnNamesHasFK.Length; j++)
                    {
                        // edit columnName is not string is variable
                        if (_ColumnNamesHasFK[j] == columnName)
                        {
                            sb.AppendLine($"    this.{_TablesNameHasFK[j]}Info = cls{_TablesNameHasFK[j]}.FindBy{columnName}(this.{columnName});");
                        }
                    }
                }
            }

            */

                            // The New Algotithm More Optimze Use Dicationary 


            // إنشاء خريطة للمفاتيح الخارجية لسهولة الوصول
            var foreignKeyMap = _ColumnNamesHasFK
                .Select((fkColumn, index) => new { fkColumn, tableName = _TablesNameHasFK[index] })
                .ToDictionary(x => x.fkColumn, x => x.tableName);

            // تعيين الحقول الداخلية من المعاملات
            foreach (var columnName in _Columns)
            {
                sb.AppendLine($"            this.{columnName} = {columnName};");

                // إذا كان العمود مفتاح خارجي، أضف البحث
                if (foreignKeyMap.TryGetValue(columnName, out var tableName))
                {
                    sb.AppendLine($"            this.{tableName}Info = cls{tableName}.FindBy{columnName}(this.{columnName});");
                }
            }

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("            Mode = enMode.Update;");

            // Closing the constructor
            sb.AppendLine("        }");

            return sb.ToString();
        }

        // this is methode is not completed
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


        public clsGlobal.enTypeRaisons CreateBusinessLayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}.cs");


            // Methods in dataAccesLayer

            //{AddGetTableInfoByIDMethod()}
            //
            //{AddGetAllDataMethod()}
            //
            //{AddAddingNewRecordMethod()}
            //
            //{AddUpdatingRecordMethod()}
            //
            //{AddDeleteByIDMethod()}
            //
            //{AddSearchMethod()}


            string code = @$"
using System;
using System.Data;
using {clsGlobal.DataBaseName}_DataLayer;

namespace {clsGlobal.DataBaseName}_BusinessLayer
{{
    public class cls{_TableName}
    {{
        #nullable enable

        public enum enMode {{ AddNew = 0, Update = 1 }};
        public enMode Mode = enMode.AddNew;

{AddAllFields(_Columns, _DataTypes, _NullibietyColumns, _ColumnNamesHasFK, _TablesNameHasFK)}

{AddNormalConstructor(_Columns, _DataTypes, _NullibietyColumns, _TableName)}

{AddUpdateConstructor(_Columns,_DataTypes, _NullibietyColumns, _TableName, _ColumnNamesHasFK, _TablesNameHasFK)}

{AddAddingNewRow(_Columns, _TableName)}



    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateBusinessLayerFile(string filePath, string TableName, string[] Columns,
            string[] DataTypes, bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, bool AddingStaticMethods)
        {
            clsCreateBusinessLayerFile Files = new clsCreateBusinessLayerFile(filePath, TableName, Columns, DataTypes,
                                                                        NullibietyColumns, ColumnNamesHasFK, TablesNameHasFK, AddingStaticMethods);

            return Files.CreateBusinessLayerFile();
        }


    }
}
