// -----------------------------------------------------------------------
// <copyright file="CSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Controllers.Design
{
    public class EfDesignerControllerGenerator : AbstractEfDesignerControllerGenerator
    {
        public EfDesignerControllerGenerator(CodeGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        public override string GenerateClass(IEntityType entityType, string @namespace, bool useSwagger)
        {
            Provider.Builder.Clear();
            GenerateFileHeader();

            WriteLine($"namespace {@namespace}");
            using (OpenBlock())
            {

            }

            return Provider.Builder.ToString();
        }
    }
}
