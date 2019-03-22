// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpDbContextGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using TypeBaseExtensions = Microsoft.EntityFrameworkCore.Metadata.Internal.TypeBaseExtensions;

namespace Microsoft.EntityFrameworkCore.Design.Context
{
    public abstract class AbstractCSharpDbContextGenerator : AbstractIndentedCodeWriter, ICSharpDbContextGenerator
    {
        protected AbstractCSharpDbContextGenerator(IDbContextServiceProvider serviceProvider) : base(serviceProvider.Provider)
        {
            Pluralizer = serviceProvider.Pluralizer;
            Helper = serviceProvider.Helper;
            AnnotationCodeGenerator = serviceProvider.AnnotationCodeGenerator;
            AnnotationsBuilder = serviceProvider.AnnotationsBuilder;

        }

        protected IPluralizer Pluralizer { get; }

        protected ICSharpHelper Helper { get; }

        protected IAnnotationsBuilder AnnotationsBuilder { get; }

        protected IAnnotationCodeGenerator AnnotationCodeGenerator { get; }


        public virtual string WriteCode(IModel model,
            string @namespace,
            string contextName,
            string connectionString,
            bool useDataAnnotations,
            bool suppressConnectionStringWarning)
        {
            GenerateFileHeader();

            WriteLine("using Microsoft.EntityFrameworkCore;");
            WriteLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            WriteLine();
            WriteLine($"namespace {@namespace}");
            using (OpenBlock())
            {
                GenerateClass(model, contextName, connectionString);
            }

            return StringBuilderProvider.Builder.ToString();
        }



        protected abstract void GenerateOnModelCreating(IModel model);

        protected abstract void GenerateOnConfiguring(string connectionString);

        protected abstract void GenerateDbSets(IModel model);

        protected abstract void GenerateConstructors(string contextName);

        protected virtual void GenerateClass(IModel model,
            string contextName,
            string connectionString)
        {
            WriteLine($"public partial class {contextName} : DbContext");
            using (OpenBlock())
            {
                GenerateConstructors(contextName);
                GenerateDbSets(model);
                GenerateOnConfiguring(connectionString);
                GenerateOnModelCreating(model);
                GenerateConfigurationClasses(model);
            }
        }


        protected virtual void GenerateConfigurationClasses(IModel model)
        {
            WriteLine();
            foreach (var entityType in model.GetEntityTypes())
            {
                GenerateConfigurationClass(model, entityType);
            }
        }

        protected virtual void GenerateConfigurationClass(IModel model, IEntityType entityType)
        {
            WriteLine();
            WriteLine($"private class {entityType.Name}Configuration  : IEntityTypeConfiguration<{entityType.Name}>");
            using (OpenBlock())
            {
                WriteLine($"public void Configure(EntityTypeBuilder<{entityType.Name}> builder)");
                using (OpenBlock())
                {
                    var indexes = entityType.GetIndexes().ToImmutableArray();
                    var properties = entityType.GetProperties().ToImmutableArray();
                    var foreignKeys = entityType.GetForeignKeys().ToImmutableArray();

                    WriteLine();

                    if (indexes.Length > 0)
                    {
                        WriteLine("// Indexes");
                        foreach (IIndex index in indexes)
                        {
                            GenerateIndex(index);
                        }

                        WriteLine();
                    }

                    if (properties.Length > 0)
                    {
                        WriteLine("// Properties");
                        foreach (IProperty property in entityType.GetProperties())
                        {
                            GenerateProperty(entityType, property);
                        }

                        WriteLine();
                    }

                    if (foreignKeys.Length > 0)
                    {
                        WriteLine("// Foreign Keys");
                        foreach (IForeignKey foreignKey in entityType.GetForeignKeys())
                        {
                            GenerateRelationship(foreignKey);
                        }

                        WriteLine();
                    }
                }
            }
        }

        protected abstract void GenerateRelationship(IForeignKey foreignKey);

        protected abstract void GenerateProperty(IEntityType entityType, IProperty property);

        protected abstract void GenerateIndex(IIndex index);
    }
}
