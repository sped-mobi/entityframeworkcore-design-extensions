// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2019 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Design.Controllers
{
    public abstract class AbstractCSharpControllerGenerator : AbstractIndentedCodeWriter, ICSharpControllerGenerator
    {
        public abstract string GenerateClass(IEntityType entityType, string @namespace, bool useSwagger);

        protected AbstractCSharpControllerGenerator(IDbContextServiceProvider provider) : base(provider.Provider)
        {
            Pluralizer = provider.Pluralizer;
            CSharpHelper = provider.Helper;
            AnnotationCodeGenerator = provider.AnnotationCodeGenerator;
            AnnotationsBuilder = provider.AnnotationsBuilder;
        }

        protected IPluralizer Pluralizer { get; }

        protected ICSharpHelper CSharpHelper { get; }

        protected IAnnotationCodeGenerator AnnotationCodeGenerator { get; }

        protected IAnnotationsBuilder AnnotationsBuilder { get; }
    }
}
