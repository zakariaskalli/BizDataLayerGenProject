using BizDataLayerGen.GeneralClasses;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BizDataLayerGen.Creation_APIs
{
    public class clsCreateAPIController
    {
        private string _apiLayerPath;
        private string _TableName;
        private string[] _Columns;
        private string[] _DataTypes;
        private bool[] _NullibietyColumns;
        private string[] _ColumnNamesHasFK;
        private string[] _TablesNameHasFK;
        private string[] _ReferencedColumn;
        private bool _AddPaggination;

        public clsCreateAPIController(string apiLayerPath, string TableName, string[] Columns, string[] DataTypes,
                                  bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[]
                                  ReferencedColumn,bool AddPaggination=true)
        {
            this._apiLayerPath = apiLayerPath;
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
            this._AddPaggination = AddPaggination;
        }

        // Returns appropriate Route Constraint based on C# Data Type
        private static string GetRouteConstraint(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "int":
                case "short":
                case "long":
                case "byte":
                    return ":int:min(1)";

                case "guid":
                    return ":guid";

                default:
                    return "";
            }
        }

        public string CreateGetAllEndPoint(string _TableName)
        {
            string entityNameLowerPlural = _TableName.Pluralize().ToLower();

            string EndPointWithPaggination = $@"
                [HttpGet("""", Name = ""GetAll{_TableName.Pluralize()}"")]
                [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<cls{_TableName.Singularize()}DTO>))]
                public async Task<ActionResult<PagedResultDTO<cls{_TableName.Singularize()}DTO>>> GetAll{_TableName.Pluralize()}([FromQuery] {_TableName.Singularize()}QueryParameters query)
                {{
                     PagedResultDTO<cls{_TableName.Singularize()}DTO>  {entityNameLowerPlural} = await cls{_TableName.Singularize()}.GetAll{_TableName.Pluralize()}Async(query.PageNumber, query.PageSize);
        
                    if ({entityNameLowerPlural} == null || !{entityNameLowerPlural}.Items.Any())
                    {{
                        return Ok(Enumerable.Empty<cls{_TableName.Singularize()}DTO>());
                    }}
        
                    return Ok({entityNameLowerPlural});
            }}";


            string EndPoint = $@"
            [HttpGet("""", Name = ""GetAll{_TableName.Pluralize()}"")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<cls{_TableName.Singularize()}DTO>))]
            public async Task<ActionResult<IEnumerable<cls{_TableName.Singularize()}DTO>>> GetAll{_TableName.Pluralize()}())
            {{
            List<cls{_TableName.Singularize()}DTO> {entityNameLowerPlural} = await cls{_TableName.Singularize()}.GetAll{_TableName.Pluralize()}Async();
        
            if ({entityNameLowerPlural} == null || !{entityNameLowerPlural}.Any())
            {{
                return Ok(Enumerable.Empty<cls{_TableName.Singularize()}DTO>());
            }}
        
            return Ok({entityNameLowerPlural});
            }}";

            return _AddPaggination ? EndPointWithPaggination : EndPoint ;
        }

        public string CreateGetByIDEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string idParamName = char.ToLower(IdColumnName[0]) + IdColumnName.Substring(1);

            string routeConstraint = GetRouteConstraint(IdColumnDataType);
            string EndPoint = $@"
            [HttpGet(""{{{idParamName}{routeConstraint}}}"", Name = ""Get{_TableName.Singularize()}ById"")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(cls{_TableName.Singularize()}DTO))]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<cls{_TableName.Singularize()}DTO>> Get{_TableName.Singularize()}ById([FromRoute] {IdColumnDataType} {idParamName})
            {{
                    if (!ModelState.IsValid)
            {{
                return BadRequest(ModelState);
            }}
        
            cls{_TableName.Singularize()}? {_TableName.Singularize().ToLower()} = await cls{_TableName.Singularize()}.FindBy{IdColumnName}Async({idParamName});
        
            if ({_TableName.Singularize().ToLower()} == null)
            {{
                return NotFound(""{_TableName.Singularize()} with ID "" + {idParamName} + "" was not found."");
            }}
        
            return Ok({_TableName.Singularize().ToLower()}.Data);
        }}";

            return EndPoint;
        }

        public string CreateAddNewEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string idParamName = char.ToLower(IdColumnName[0]) + IdColumnName.Substring(1);
            string entitySingular = _TableName.Singularize();
            string entityLowerSingular = entitySingular.ToLower();

            string EndPoint = $@"
        [HttpPost(Name = ""AddNew{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(cls{entitySingular}DTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<cls{entitySingular}DTO>> AddNew{entitySingular}([FromBody] cls{entitySingular}DTO {entityLowerSingular}DTO)
        {{
            if (!ModelState.IsValid || {entityLowerSingular}DTO == null)
            {{
                return BadRequest(ModelState);
            }}

            bool isCreated = await cls{entitySingular}.AddNew{entitySingular}Async({entityLowerSingular}DTO);

            if (isCreated == false)
            {{
                return BadRequest(""Failed to create new {entitySingular}."");
            }}

            return CreatedAtRoute(""Get{entitySingular}ById"", new {{ {idParamName} = {entityLowerSingular}DTO.{IdColumnName} }}, {entityLowerSingular}DTO);
        }}";

            return EndPoint;
        }

        public string CreateUpdateEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string idParamName = char.ToLower(IdColumnName[0]) + IdColumnName.Substring(1);
            string entitySingular = _TableName.Singularize();
            string entityLowerSingular = entitySingular.ToLower();
            string routeConstraint = GetRouteConstraint(IdColumnDataType);

            string EndPoint = $@"
        [HttpPut(""{{{idParamName}{routeConstraint}}}"", Name = ""Update{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(cls{entitySingular}DTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<cls{entitySingular}DTO>> Update{entitySingular}(
            [FromRoute] {IdColumnDataType} {idParamName},
            [FromBody] cls{entitySingular}DTO {entityLowerSingular}DTO)
        {{
            if (!ModelState.IsValid || {entityLowerSingular}DTO == null || !{idParamName}.Equals({entityLowerSingular}DTO.{IdColumnName}))
            {{
                return BadRequest(ModelState);
            }}

            if (!await cls{entitySingular}.Update{entitySingular}ByIDAsync({entityLowerSingular}DTO))
            {{
                return NotFound(""{entitySingular} with ID "" + {idParamName} + "" was not found."");
            }}

            return Ok({entityLowerSingular}DTO);
        }}";

            return EndPoint;
        }

        public string CreateDeleteEndPoint(string _TableName, string IdColumnName, string IdColumnDataType)
        {
            string idParamName = char.ToLower(IdColumnName[0]) + IdColumnName.Substring(1);
            string entitySingular = _TableName.Singularize();
            string routeConstraint = GetRouteConstraint(IdColumnDataType);

            string EndPoint = $@"
        [HttpDelete(""{{{idParamName}{routeConstraint}}}"", Name = ""Delete{entitySingular}"")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete{entitySingular}([FromRoute] {IdColumnDataType} {idParamName})
        {{
            if (!ModelState.IsValid)
            {{
                return BadRequest(ModelState);
            }}
        
            if (!await cls{entitySingular}.Delete{entitySingular}Async({idParamName}))
            {{
                return NotFound(""{entitySingular} with ID "" + {idParamName} + "" was not found."");
            }}
        
            return NoContent();
        }}";

            return EndPoint;
        }

        public string CreateSearchEndPoint(string _TableName)
        {
            string entityNameLowerPlural = _TableName.Pluralize().ToLower();

            string EndPoint = $@"
        [HttpGet(""search"", Name = ""Search{_TableName.Pluralize()}"")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<cls{_TableName.Singularize()}DTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<cls{_TableName.Singularize()}DTO>>> Search{_TableName.Pluralize()}(
            [FromQuery] cls{_TableName.Singularize()}.{_TableName.Singularize()}Column column,
            [FromQuery] string value,
            [FromQuery] cls{_TableName.Singularize()}.SearchMode mode = cls{_TableName.Singularize()}.SearchMode.Anywhere)
        {{
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(value))
            {{
                return BadRequest(""Search value cannot be empty."");
            }}

            List<cls{_TableName.Singularize()}DTO>? {entityNameLowerPlural} = await cls{_TableName.Singularize()}.SearchDataAsync(column, value, mode);

            if ({entityNameLowerPlural} == null || !{entityNameLowerPlural}.Any())
            {{
                return Ok(Enumerable.Empty<cls{_TableName.Singularize()}DTO>());
            }}

            return Ok({entityNameLowerPlural});
        }}";

            return EndPoint;
        }

        public async Task<clsGlobal.enTypeRaisons> CreateAPILayerFile()
        {
            var controllerFolderPath = Path.Combine(_apiLayerPath, "Controllers","V1");
            if (!Directory.Exists(controllerFolderPath))
            {
                Directory.CreateDirectory(controllerFolderPath);
            };

            // Define the full path for the file
            string fullPath = Path.Combine(controllerFolderPath, $"cls{_TableName}Controller.cs");

            // Names Of Methods to generate the EndPoints
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
using {clsGlobal.ProjectName}.DTO.Common;
using {clsGlobal.ProjectName}.DTO.{_TableName.Singularize()};
using {clsGlobal.ProjectName}_BusinessLayer;

namespace {clsGlobal.ProjectName}Api.Controllers
{{

    [ApiController]
    [Route(""api/v{{version:apiVersion}}/[controller]"")]

    public class {_TableName.Singularize()}Controller : ControllerBase // Declare the controller class inheriting from ControllerBase.
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
            await Task.Run(() => File.WriteAllText(fullPath, code));

            await CreateProgramAndConfigurationFiles(_apiLayerPath);

            return clsGlobal.enTypeRaisons.enPerfect;
        }


        public static async Task<clsGlobal.enTypeRaisons> CreateAPILayerFile(string filePath, string TableName, string[] Columns, string[] DataTypes,
                                  bool[] NullibietyColumns, string[] ColumnNamesHasFK, string[] TablesNameHasFK, string[]
                                  ReferencedColumn)
        {
            clsCreateAPIController Files = new clsCreateAPIController(filePath, TableName, Columns, DataTypes, NullibietyColumns,
                ColumnNamesHasFK, TablesNameHasFK, ReferencedColumn);

            await CreateProgramAndConfigurationFiles(filePath);

            return await Files.CreateAPILayerFile();
        }
        

        /// <summary>
        /// Generates the Program.cs and modular Configuration files (DependencyInjection.cs) for the API project.
        /// </summary>
        /// <param name="apiFolderPath">The root directory path of the generated API project.</param>
        /// <returns>Execution status enum indicating successful creation.</returns>
        
        

        public static async Task<clsGlobal.enTypeRaisons> CreateProgramAndConfigurationFiles(string apiFolderPath)
        {
            // Create the Configurations directory inside the root API directory
            string configFolderPath = Path.Combine(apiFolderPath, "Configurations");

            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }

            // 1. Source code for Configurations/DependencyInjection.cs
            string configCode = @$"using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace {clsGlobal.ProjectName}Api.Configurations
{{
    public static class DependencyInjection
    {{
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {{
            // 1. Configure Controllers & JSON Options (camelCase & String Enums)
            services.AddControllers().AddJsonOptions(options =>
            {{
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            }});

            // 2. Configure API Versioning
            services.AddApiVersioning(options =>
            {{
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }})
            .AddMvc(options =>
            {{
                options.Conventions.Add(new VersionByNamespaceConvention());
            }})
            .AddApiExplorer(options =>
            {{
                options.GroupNameFormat = ""'v'V"";
                options.SubstituteApiVersionInUrl = true;
            }});

            // Enable API Explorer for Swagger
            services.AddEndpointsApiExplorer();

            // 3. Configure Swagger Generation & Custom Schema Handling
            services.AddSwaggerGen(options =>
            {{
                options.DescribeAllParametersInCamelCase();

                // Resolve cross-naming conflicts for Nested Types and Generic Models
                options.CustomSchemaIds(type =>
                {{
                    // Handle Nested Types (e.g., clsTasks.SearchMode)
                    if (type.IsNested)
                    {{
                        return $""{{type.DeclaringType.Name}}.{{type.Name}}"";
                    }}

                    // Handle Generic Types (e.g., ActionResult<List<T>>)
                    if (type.IsGenericType)
                    {{
                        var genericArguments = string.Join("""", type.GetGenericArguments().Select(t => t.Name));
                        return $""{{type.Name[..type.Name.IndexOf('`')]}}{{genericArguments}}"";
                    }}

                    // Standard types
                    return type.Name;
                }});
            }});

            return services;
        }}
    }}
}}";

            // 2. Source code for root Program.cs
            string programCode = @$"using Microsoft.AspNetCore.Builder;
using {clsGlobal.ProjectName}Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using modular Configuration Extension
builder.Services.AddApiServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{{
    // Run Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();";

            // Save the generated files to the target directories
            string configFilePath = Path.Combine(configFolderPath, "DependencyInjection.cs");
            string programFilePath = Path.Combine(apiFolderPath, "Program.cs");

            await Task.Run(() => File.WriteAllText(configFilePath, configCode));
            await Task.Run(() => File.WriteAllText(programFilePath, programCode));

            return clsGlobal.enTypeRaisons.enPerfect;
        }


    }
}

