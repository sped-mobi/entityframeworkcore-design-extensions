// -----------------------------------------------------------------------
// <copyright file="CSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;

namespace Microsoft.EntityFrameworkCore.Controllers.Design
{
    public class EfDesignerControllerGenerator : AbstractEfDesignerControllerGenerator
    {
        public EfDesignerControllerGenerator(CodeGeneratorDependencies dependencies) : base(dependencies)
        {
        }



        protected override void WriteUsings()
        {
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using System.Linq;");
            WriteLine("using System.Threading.Tasks;");
            WriteLine("using Microsoft.AspNetCore.Mvc;");
            WriteLine("using System.Threading;");
            WriteLine("using Newtonsoft.Json;");
            WriteLine("using System.Diagnostics;");
            WriteLine();
        }

        protected override void WriteGetMethod(string entityName, string viewModelName)
        {
            WriteLine("[HttpGet]");
            WriteLine($"[Produces(typeof(List<{viewModelName}>))]");
            WriteLine($"public async Task<ActionResult<List<{viewModelName}>>> Get(CancellationToken ct = default)");
            using (OpenBlock())
            {
                WriteLine("try");
                using (OpenBlock())
                {
                    WriteLine($"return new ObjectResult(await _supervisor.GetAll{entityName}Async(ct));");
                }
                WriteLine("catch (Exception ex)");
                using (OpenBlock())
                {
                    WriteLine("return StatusCode(500, ex);");
                }
            }
        }

        protected override void WriteGetByIdMethod(string key, string entityName, string viewModelName)
        {
            WriteLine("[HttpGet(\"{id}\")]");
            WriteLine($"[Produces(typeof({viewModelName}))]");
            WriteLine($"public async Task<ActionResult<{viewModelName}>> Get({key} id, CancellationToken ct = default)");
            using (OpenBlock())
            {
                WriteLine("try");
                using (OpenBlock())
                {
                    WriteLine($"var {entityName} = await _supervisor.Get{entityName}ByIdAsync(id, ct);");
                    WriteLine($"if ( {entityName} == null)");
                    using (OpenBlock())
                    {
                        WriteLine("return NotFound();");
                    }
                    WriteLine($"return Ok({entityName});");
                }
                WriteLine("catch (Exception ex)");
                using (OpenBlock())
                {
                    WriteLine("return StatusCode(500, ex);");
                }
            }
        }

        protected override void WritePostMethod(string entityName, string viewModelName)
        {
            WriteLine("[HttpPost]");
            WriteLine($"[Produces(typeof({viewModelName}))]");
            WriteLine($"public async Task<ActionResult<{viewModelName}>> Post([FromBody] {viewModelName} input, CancellationToken ct = default)");
            using (OpenBlock())
            {
                WriteLine("try");
                using (OpenBlock())
                {
                    WriteLine("if (input == null)");
                    WriteLine("return BadRequest();");
                    WriteLine($"return StatusCode(201, await _supervisor.Add{entityName}Async(input, ct));");
                }
                WriteLine("catch (Exception ex)");
                using (OpenBlock())
                {
                    WriteLine("return StatusCode(500, ex);");
                }
            }
        }

        protected override void WritePutMethod(string key, string entityName, string viewModelName)
        {
            WriteLine();
            WriteLine("[HttpPut(\"{id}\")]");
            WriteLine($"[Produces(typeof({viewModelName}))]");
            WriteLine($"public async Task<ActionResult<{viewModelName}>> Put({key} id, [FromBody] {viewModelName} input, CancellationToken ct = default)");
            using (OpenBlock())
            {
                WriteLine("try");
                using (OpenBlock())
                {
                    WriteLine("if (input == null)");
                    WriteLine("return BadRequest();");
                    WriteLine($"if (await _supervisor.Get{entityName}ByIdAsync(id, ct) == null)");
                    using (OpenBlock())
                    {
                        WriteLine("return NotFound();");
                    }
                    WriteLine("var errors = JsonConvert.SerializeObject(ModelState.Values");
                    PushIndent();
                    WriteLine(".SelectMany(state => state.Errors)");
                    WriteLine(".Select(error => error.ErrorMessage));");
                    PopIndent();
                    WriteLine("Debug.WriteLine(errors);");
                    WriteLine($"if (await _supervisor.Update{entityName}Async(input, ct))");
                    using (OpenBlock())
                    {
                        WriteLine("return Ok(input);");
                    }
                    WriteLine("return StatusCode(500);");
                }
                WriteLine("catch (Exception ex)");
                using (OpenBlock())
                {
                    WriteLine("return StatusCode(500, ex);");
                }
            }
        }

        protected override void WriteDeleteMethod(string entityName, string key)
        {
            WriteLine("[HttpDelete(\"{id}\")]");
            WriteLine("[Produces(typeof(void))]");
            WriteLine($"public async Task<ActionResult> Delete({key} id, CancellationToken ct = default)");
            using (OpenBlock())
            {
                WriteLine("try");
                using (OpenBlock())
                {
                    WriteLine($"if (await _supervisor.Get{entityName}ByIdAsync(id, ct) == null)");
                    using (OpenBlock())
                    {
                        WriteLine("return NotFound();");
                    }
                    WriteLine($"if (await _supervisor.Delete{entityName}Async(id, ct))");
                    using (OpenBlock())
                    {
                        WriteLine("return Ok();");
                    }
                    WriteLine("return StatusCode(500);");
                }
                WriteLine("catch (Exception ex)");
                using (OpenBlock())
                {
                    WriteLine("return StatusCode(500, ex);");
                }
            }
        }
    }
}
