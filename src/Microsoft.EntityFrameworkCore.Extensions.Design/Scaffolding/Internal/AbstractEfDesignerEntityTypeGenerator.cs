// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpEntityTypeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public abstract class AbstractEfDesignerEntityTypeGenerator : AbstractCodeGenerator, ICSharpEntityTypeGenerator
    {
        protected AbstractEfDesignerEntityTypeGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
        {
        }

        protected bool UseDataAnnotations { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            UseDataAnnotations = true;
            Provider.Builder.Clear();
            GenerateFileHeader();
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine();
            WriteLine($"namespace {@namespace}");
            using (OpenBlock())
            {
                GenerateClass(entityType);
                WriteLine();
                GenerateInterface(entityType);
            }

            return Provider.Builder.ToString();
        }

        protected abstract void GenerateClass(IEntityType entityType);

        protected abstract void GenerateInterface(IEntityType entityType);
    }
}
