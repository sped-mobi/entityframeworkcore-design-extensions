// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2019 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Controllers.Design
{
    public abstract class AbstractEfDesignerControllerGenerator : AbstractCodeGenerator, IEfDesignerControllerGenerator
    {
        protected AbstractEfDesignerControllerGenerator(CodeGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        public abstract string GenerateClass(IEntityType entityType, string @namespace, bool useSwagger);
    }
}
