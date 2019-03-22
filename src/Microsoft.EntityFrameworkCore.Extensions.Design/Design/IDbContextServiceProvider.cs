namespace Microsoft.EntityFrameworkCore.Design
{
    public interface IDbContextServiceProvider
    {
        IStringBuilderProvider Provider { get; }

        IPluralizer Pluralizer { get; }

        ICSharpHelper Helper { get; }

        IAnnotationCodeGenerator AnnotationCodeGenerator { get; }

        IAnnotationsBuilder AnnotationsBuilder { get; }
    }
}
