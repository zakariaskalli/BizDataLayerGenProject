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

        private static string GetIdValidationCondition(string dataType, string CoulumnIDName)
        {
            switch (dataType.ToLower())
            {
                case "int":
                case "short":
                case "long":
                case "byte":
                    return $"{CoulumnIDName} <= 0";

                case "guid":
                    return $"{CoulumnIDName} == Guid.Empty";

                case "string":
                    return $"string.IsNullOrWhiteSpace({CoulumnIDName})";

                default:
                    return "false";
            }
        }

        public string CreateGetAllEndPoint(string _TableName)
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

        public string CreateGetByIDEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string entityNameLowerSingulare = _TableName.Singularize().ToLower();
            string EndPoint = $@"
        [HttpGet(""{{{IdColumnName}:{IdColumnDataType}}}"", Name = ""Get{_TableName}ById"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(cls{_TableName}DTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<cls{_TableName}DTO> Get{_TableName.Singularize()}ById([FromRoute] {IdColumnDataType} {IdColumnName})
        {{
            if ({GetIdValidationCondition(IdColumnDataType, IdColumnName)})
            {{
                return BadRequest(""Invalid {_TableName.Singularize()} ID."");
            }}
        
            cls{_TableName}? {_TableName.Singularize().ToLower()} = cls{_TableName}.FindBy{IdColumnName}({IdColumnName});
        
            if ({_TableName.Singularize().ToLower()} == null)
            {{
                return NotFound(""{_TableName} with ID "" + {IdColumnName} + "" was not found."");
            }}
        
            return Ok({_TableName.Singularize().ToLower()}.Data);
        }}";

            return EndPoint;
        }

        public string CreateAddNewEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string entitySingular = _TableName.Singularize();
            string entityLowerSingular = entitySingular.ToLower();
            string IdColumnNameLower = IdColumnName.ToLower(); // المعرف مثل ProjectID

            string EndPoint = $@"
        [HttpPost(Name = ""AddNew{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(cls{_TableName}DTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<cls{_TableName}DTO> AddNew{entitySingular}([FromBody] cls{_TableName}DTO {entityLowerSingular}DTO)
        {{
            if ({entityLowerSingular}DTO == null)
            {{
                return BadRequest(""Invalid data provided."");
            }}

            bool isCreated = cls{_TableName}.AddNew{_TableName}({entityLowerSingular}DTO);

            if (isCreated == false)
            {{
                return BadRequest(""Failed to create new {entitySingular}."");
            }}

            return CreatedAtRoute(""Get{_TableName}ById"", new {{ {IdColumnName} = {entityLowerSingular}DTO.{IdColumnName} }}, {entityLowerSingular}DTO);
            
        }}";

            return EndPoint;
        }

        public string CreateUpdateEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string entitySingular = _TableName.Singularize();
            string entityLowerSingular = entitySingular.ToLower();

            string EndPoint = $@"
        [HttpPut(""{{{IdColumnName}:{IdColumnDataType}}}"", Name = ""Update{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(cls{_TableName}DTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<cls{_TableName}DTO> Update{entitySingular}(
            [FromRoute] {IdColumnDataType} {IdColumnName},
            [FromBody] cls{_TableName}DTO {entityLowerSingular}DTO)
        {{
            if ({GetIdValidationCondition(IdColumnDataType, IdColumnName)})
            {{
                return BadRequest(""Invalid {entitySingular} ID."");
            }}

            if ({entityLowerSingular}DTO == null || {IdColumnName} != {entityLowerSingular}DTO.{IdColumnName})
            {{
                return BadRequest(""Invalid data provided."");
            }}

            if (!cls{_TableName}.Update{entitySingular}ByID({entityLowerSingular}DTO))
            {{
                return NotFound(""{entitySingular} with ID "" + {IdColumnName} + "" was not found."");
            }}

            return Ok({entityLowerSingular}DTO);
        }}";

            return EndPoint;
        }

        public string CreateDeleteEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string entitySingular = _TableName.Singularize();

            string EndPoint = $@"
        [HttpDelete(""{{{IdColumnName}:{IdColumnDataType}}}"", Name = ""Delete{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete{entitySingular}([FromRoute] {IdColumnDataType} {IdColumnName})
        {{
            if ({GetIdValidationCondition(IdColumnDataType, IdColumnName)})
            {{
                return BadRequest(""Invalid {entitySingular} ID."");
            }}
        
            if (!cls{_TableName}.Delete{entitySingular}({IdColumnName}))
            {{
                return NotFound(""{entitySingular} with ID "" + {IdColumnName} + "" was not found أو لا يمكن حذفه لارتباطه ببيانات أخرى."");
            }}
        
            return Ok(new {{ message = ""{entitySingular} deleted successfully."" }});
        }}";

            return EndPoint;
        }

        public string CreateSearchEndPoint(string _TableName)
        {
            string entitySingular = _TableName.Singularize();
            string entityNameLowerPlural = _TableName.Pluralize().ToLower();

            string EndPoint = $@"
        [HttpGet(""search"", Name = ""Search{_TableName.Pluralize()}"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<cls{_TableName}DTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<cls{_TableName}DTO>> Search{_TableName.Pluralize()}(
            [FromQuery] cls{_TableName}.{_TableName}Column column,
            [FromQuery] string value,
            [FromQuery] cls{_TableName}.SearchMode mode = cls{_TableName}.SearchMode.Anywhere)
        {{
            if (string.IsNullOrWhiteSpace(value))
            {{
                return BadRequest(""Search value cannot be empty."");
            }}

            List<cls{_TableName}DTO>? {entityNameLowerPlural} = cls{_TableName}.SearchData(column, value, mode);

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

            string StringGetByID = CreateGetByIDEndPoint(_TableName, _Columns[0], _DataTypes[0]);

            string StringAddNew = CreateAddNewEndPoint(_TableName, _Columns[0], _DataTypes[0]);

            string StringUpdate = CreateUpdateEndPoint(_TableName, _Columns[0], _DataTypes[0]);

            string StringDelete = CreateDeleteEndPoint(_TableName, _Columns[0], _DataTypes[0]);

            string StringSearch = CreateSearchEndPoint(_TableName);

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

        {StringGetByID}

        {StringAddNew}

        {StringUpdate}

        {StringDelete}

        {StringSearch}

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
