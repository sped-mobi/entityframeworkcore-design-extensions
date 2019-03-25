// -----------------------------------------------------------------------
// <copyright file="AbstractCodeGenerator.cs" company="Sped Mobi">
//     Copyright © 2019 Sped Mobi All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design.Internal;

namespace Microsoft.EntityFrameworkCore.Design
{
    public abstract class AbstractCodeGenerator : AbstractIndentedCodeWriter
    {
        protected AbstractCodeGenerator(CodeGeneratorDependencies depenencies) : base(depenencies.Provider)
        {
            Pluralizer = depenencies.Pluralizer;
            Helper = depenencies.Helper;
            AnnotationCodeGenerator = depenencies.AnnotationCodeGenerator;

        }


        public IPluralizer Pluralizer { get; set; }

        public ICSharpHelper Helper { get; set; }

        public IAnnotationCodeGenerator AnnotationCodeGenerator { get; set; }
    }
}
