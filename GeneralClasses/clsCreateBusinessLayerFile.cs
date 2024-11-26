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
        public clsCreateBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            this._filePath = filePath;
            this._TableName = TableName;
            this._Columns = Columns;
            this._DataTypes = DataTypes;
            this._NullibietyColumns = NullibietyColumns;
        }

        public string AddAllFields(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns)
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

        public string AddUpdateConstructor(string[] _Columns, string[] _DataTypes, bool[] _NullibietyColumns, string _TableName)
        {
            // Ensure that the arrays are of the same length
            
            //if (_Columns.Length != _DataTypes.Length || _Columns.Length != _NullibietyColumns.Length)
            //{
            //    throw new ArgumentException("All arrays must have the same length.");
            //}


            StringBuilder sb = new StringBuilder();

            // Constructor signature with parameters
            sb.AppendLine($"private {_TableName}(");

            // Loop through columns and generate constructor parameters
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];

                // Add parameter to the constructor
                sb.AppendLine($"    {dataType} {columnName},");
            }

            // Remove the last comma
            sb.Length--;

            // Close the parameter list and open the constructor body
            sb.AppendLine(")");

            sb.AppendLine("{");

            // Loop through the columns and generate the assignments to fields
            for (int i = 0; i < _Columns.Length; i++)
            {
                string columnName = _Columns[i];
                string dataType = _DataTypes[i];

                sb.AppendLine($"    this.{columnName} = {columnName};");
            }

            // Add the additional logic for nullable fields and other specific assignments
            sb.AppendLine("    this.CountryInfo = clsCountry.Find(NationalityCountryID);");
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
             

            string code = @"
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

{AddAllFields(_Columns, _DataTypes, _NullibietyColumns)}

{AddNormalConstructor(_Columns, _DataTypes, _NullibietyColumns, _TableName)}
        
{}


    }}
}}
";

            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }

        public static clsGlobal.enTypeRaisons CreateBusinessLayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes, bool[] NullibietyColumns)
        {
            clsCreateBusinessLayerFile Files = new clsCreateBusinessLayerFile(filePath, TableName, Columns, DataTypes, NullibietyColumns);

            return Files.CreateBusinessLayerFile();
        }


    }
}
