namespace Microsoft.EntityFrameworkCore.Design.Impl
{
    public class DbContextServiceProvider : IDbContextServiceProvider
    {
        public DbContextServiceProvider(
            IStringBuilderProvider provider,
            IPluralizer pluralizer,
            ICSharpHelper helper,
            IAnnotationsBuilder annotationsBuilder,
            IAnnotationCodeGenerator annotationCodeGenerator)
        {
            Provider = provider;
            Pluralizer = pluralizer;
            Helper = helper;
            AnnotationsBuilder = annotationsBuilder;
            AnnotationCodeGenerator = annotationCodeGenerator;
        }

        public IStringBuilderProvider Provider { get; }

        public IPluralizer Pluralizer { get; }

        public ICSharpHelper Helper { get; }

        public IAnnotationCodeGenerator AnnotationCodeGenerator { get; }

        public IAnnotationsBuilder AnnotationsBuilder { get; }
    }
}
