using BizDataLayerGen.DataAccessLayer;
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
    public class clsCreateBusinessLayerFile
    {
        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private string[] _ColumnNamesHasFK;
        private string[] _TablesNameHasFK;
        public clsCreateBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                    bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK)
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
        }

        public string AddAllFields(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {
            // Ensure that the arrays are of the same length

            //if (_Columns.Length != _DataTypes.Length || _Columns.Length != _NullibietyColumns.Length)
            //{
            //    throw new ArgumentException("All arrays must have the same length.");
            //}

            StringBuilder sb = new StringBuilder();

            // Loop through all the columns and create the properties dynamically
            for (int i = 0; i < _Columns.Length; i++)
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

            return sb.ToString();
        }

        public string AddNormalConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            // Ensure that the arrays are of the same length
            
            //if (_Columns.Length != _DataTypes.Length || _Columns.Length != _NullibietyColumns.Length)
            //{
            //    throw new ArgumentException("All arrays must have the same length.");
            //}

            StringBuilder sb = new StringBuilder();

            // Constructor signature
            sb.AppendLine($"public cls{_TableName}()");

            // Constructor body
            sb.AppendLine("{");

            // Loop through all the columns and generate the assignments
            for (int i = 0; i < _Columns.Length; i++)
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

            // Closing the constructor
            sb.AppendLine("    Mode = enMode.AddNew;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public string AddUpdateConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {
            // Ensure that the arrays are of the same length
            
            //if (_Columns.Length != _DataTypes.Length || _Columns.Length != _NullibietyColumns.Length)
            //{
            //    throw new ArgumentException("All arrays must have the same length.");
            //}


            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"private cls{_TableName}(");

            // Loop through columns and generate constructor parameters
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i]; ;
                string dataType = _DataTypes[i];

                // Add parameter to the constructor
                sb.Append($" {dataType} {columnName},");
            }

            int x = 1;

            // Remove the last comma
            sb.Remove(sb.Length - 1, 1);


            // Close the parameter list and open the constructor body
            sb.AppendLine(")");

            sb.AppendLine("{");

            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];

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

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("    Mode = enMode.Update;");

            // Closing the constructor
            sb.AppendLine("}");

            return sb.ToString();
        }

        // this is methode is not completed
        public string AddAddingNewRow(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName,
            string[] _ColumnNamesHasFK, string[] _TablesNameHasFK)
        {
            // Ensure that the arrays are of the same length

            //if (_Columns.Length != _DataTypes.Length || _Columns.Length != _NullibietyColumns.Length)
            //{
            //    throw new ArgumentException("All arrays must have the same length.");
            //}


            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"private cls{_TableName}(");

            // Loop through columns and generate constructor parameters
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i]; ;
                string dataType = _DataTypes[i];

                // Add parameter to the constructor
                sb.Append($" {dataType} {columnName},");
            }

            int x = 1;

            // Remove the last comma
            sb.Remove(sb.Length - 1, 1);


            // Close the parameter list and open the constructor body
            sb.AppendLine(")");

            sb.AppendLine("{");

            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];

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

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("    Mode = enMode.Update;");

            // Closing the constructor
            sb.AppendLine("}");

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




    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateBusinessLayerFile(string filePath, string TableName, string[] Columns,
            string[] DataTypes, bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK)
        {
            clsCreateBusinessLayerFile Files = new clsCreateBusinessLayerFile(filePath, TableName, Columns, DataTypes,
                                                                        NullibietyColumns, ColumnNamesHasFK, TablesNameHasFK);

            return Files.CreateBusinessLayerFile();
        }


    }
}
