// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpEntityTypeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Microsoft.EntityFrameworkCore.Design.Entities
{
    public abstract class AbstractCSharpEntityTypeGenerator : AbstractIndentedCodeWriter, ICSharpEntityTypeGenerator
    {
        public AbstractCSharpEntityTypeGenerator(IDbContextServiceProvider serviceProvider) : base(serviceProvider.Provider)
        {
            Pluralizer = serviceProvider.Pluralizer;
            Helper = serviceProvider.Helper;
        }

        protected IPluralizer Pluralizer { get; }

        protected ICSharpHelper Helper { get; }

        protected bool UseDataAnnotations { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            UseDataAnnotations = true;
            StringBuilderProvider.Builder.Clear();
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

            return StringBuilderProvider.Builder.ToString();
        }

        protected abstract void GenerateClass(IEntityType entityType);

        protected abstract void GenerateInterface(IEntityType entityType);
    }
}
