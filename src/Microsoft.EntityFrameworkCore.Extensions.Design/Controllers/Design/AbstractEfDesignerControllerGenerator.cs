// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2019 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Threading;

namespace Microsoft.EntityFrameworkCore.Controllers.Design
{
    public abstract class AbstractEfDesignerControllerGenerator : AbstractCodeGenerator, IEfDesignerControllerGenerator
    {
        protected AbstractEfDesignerControllerGenerator(CodeGeneratorDependencies dependencies) : base(dependencies)
        {

        }

        public string WriteCode(IEntityType entityType, string @namespace, string contextName)
        {
            Provider.Builder.Clear();
            GenerateFileHeader();

            IKey primarykey = entityType.FindPrimaryKey();
            IProperty property = primarykey?.Properties.FirstOrDefault();
            string key = Helper.Reference(property?.ClrType) ?? "int";

            string entityName = entityType.Name;
            string viewModelName = $"{entityType.Name}ViewModel";
            string supervisorName = $"{contextName}Supervisor";
            string supervisorInterfaceName = $"I{supervisorName}";
            WriteUsings();
            WriteLine($"namespace {@namespace}");
            using (OpenBlock())
            {
                string controllerName = $"{entityType.Name}Controller";
                WriteLine("[Route(\"api/[controller]\")]");
                WriteLine($"public partial class {controllerName}");
                using (OpenBlock())
                {
                    WriteLine($"private readonly {supervisorInterfaceName} _supervisor;");
                    WriteLine();
                    WriteLine($"public {controllerName}({supervisorInterfaceName} supervisor)");
                    using (OpenBlock())
                    {
                        WriteLine("_supervisor = supervisor;");
                    }

                    WriteGetMethod(entityName, viewModelName);
                    WriteGetByIdMethod(key, entityName, viewModelName);
                    WritePostMethod(entityName, viewModelName);
                    WritePutMethod(key, entityName, viewModelName);
                    WriteDeleteMethod(entityName, key);

                }
            }

            return Provider.Builder.ToString();
        }


        protected abstract void WriteUsings();
        protected abstract void WriteGetMethod(string entityName, string viewModelName);
        protected abstract void WriteGetByIdMethod(string key, string entityName, string viewModelName);
        protected abstract void WritePostMethod(string entityName, string viewModelName);
        protected abstract void WritePutMethod(string key, string entityName, string viewModelName);
        protected abstract void WriteDeleteMethod(string entityName, string key);
    }
    
}
