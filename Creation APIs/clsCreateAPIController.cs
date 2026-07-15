using BizDataLayerGen.GeneralClasses;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BizDataLayerGen.Creation_APIs
{
    public class clsCreateAPIController
    {

        private string _filePath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private string[] _ColumnNamesHasFK;
        private string[] _TablesNameHasFK;
        private string[] _ReferencedColumn;

        public clsCreateAPIController(string filePath, string TableName, string[] Columns, string[] DataTypes,
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


        public string CreateGetAllEndPoint(string _TableName )
        {
            string entityNameLowerPlural = _TableName.Pluralize().ToLower();
            string EndPoint = $@"
        [HttpGet("""", Name = ""GetAll{_TableName.Pluralize()}"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<cls{_TableName}DTO>))]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<cls{_TableName}DTO>> GetAll{_TableName.Pluralize()}()
        {{
            List<cls{_TableName}DTO> {entityNameLowerPlural} = cls{_TableName}.GetAll{_TableName}();
        
            if ({entityNameLowerPlural} == null || !{entityNameLowerPlural}.Any())
            {{
                return Ok(Enumerable.Empty<cls{_TableName}DTO>());
            }}
        
            return Ok({entityNameLowerPlural});
        }}";

            return EndPoint;
        }


        public async Task<clsGlobal.enTypeRaisons> CreateAPILayerFile()
        {
            // Define the full path for the file
            string fullPath = Path.Combine(_filePath, $"cls{_TableName}Controller.cs");


            //Names Of Methods to generate the EndPoints
            string StringGetAllEndPoint = CreateGetAllEndPoint(_TableName);



            string code = @$"
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using {clsGlobal.ProjectName}.DTO;
using {clsGlobal.ProjectName}_BusinessLayer;
using static {clsGlobal.ProjectName}_BusinessLayer.cls{_TableName};

namespace {clsGlobal.ProjectName}Api.Controllers
{{

    [ApiController]
    [Route(""api/{_TableName.Pluralize()}"")]

    public class {_TableName}Controller : ControllerBase // Declare the controller class inheriting from ControllerBase.
    {{
        
        {StringGetAllEndPoint}



    }}    

}}
";




            // Write the code to the file
            File.WriteAllText(fullPath, code);

            return clsGlobal.enTypeRaisons.enPerfect;

        }


        public static async Task<clsGlobal.enTypeRaisons> CreateAPILayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                  bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[]
                                  ReferencedColumn)
        {
            clsCreateAPIController Files = new clsCreateAPIController(filePath, TableName, Columns, DataTypes, NullibietyColumns,
                ColumnNamesHasFK, TablesNameHasFK, ReferencedColumn);

            return await Files.CreateAPILayerFile();
        }

    }
}
