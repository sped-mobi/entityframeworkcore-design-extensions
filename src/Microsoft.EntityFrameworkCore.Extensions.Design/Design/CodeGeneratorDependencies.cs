using Microsoft.EntityFrameworkCore.Design.Internal;

namespace Microsoft.EntityFrameworkCore.Design
{
    public class CodeGeneratorDependencies
    {
        public CodeGeneratorDependencies(
            IStringBuilderProvider provider,
            ICSharpHelper helper,
            IAnnotationCodeGenerator annotationCodeGenerator,
            IPluralizer pluralizer)
        {
            Provider = provider;
            Pluralizer = pluralizer;
            Helper = helper;
            AnnotationCodeGenerator = annotationCodeGenerator;
        }

        public IStringBuilderProvider Provider { get; set; }

        public IPluralizer Pluralizer { get; set; }

        public ICSharpHelper Helper { get; set; }

        public IAnnotationCodeGenerator AnnotationCodeGenerator { get; set; }

    }
}
