// -----------------------------------------------------------------------
// <copyright file="CSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Design.Controllers
{
    public sealed class CSharpControllerGenerator : AbstractCSharpControllerGenerator
    {
        public CSharpControllerGenerator(IDbContextServiceProvider provider) : base(provider)
        {
        }

        public override string GenerateClass(IEntityType entityType, string @namespace, bool useSwagger)
        {
            StringBuilderProvider.Builder.Clear();
            GenerateFileHeader();

            WriteLine($"namespace {@namespace}");
            using (OpenBlock())
            {

            }


            return null;
        }
    }
}
