// -----------------------------------------------------------------------
// <copyright file="CSharpDbContextGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TBE = Microsoft.EntityFrameworkCore.Metadata.Internal.TypeBaseExtensions;

namespace Microsoft.EntityFrameworkCore.Design.Context
{
    public class CSharpDbContextGenerator : AbstractCSharpDbContextGenerator
    {
        public CSharpDbContextGenerator(IDbContextServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override void GenerateOnModelCreating(IModel model)
        {
            var entities = model.GetEntityTypes().ToImmutableArray();
            WriteLine();
            WriteLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
            using (OpenBlock())
            {
                foreach (var entity in entities)
                {
                    WriteLine($"modelBuilder.ApplyConfiguration(new {entity.Name}Configuration());");
                }
            }

            GenerateConfigurationClasses(model);
        }

        protected override void GenerateOnConfiguring(string connectionString)
        {
            WriteLine();
            WriteLine("protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
            using (OpenBlock())
            {
                WriteLine("if (!optionsBuilder.IsConfigured)");
                using (OpenBlock())
                {
                    WriteLine($"optionsBuilder.UseSqlServer(\"{connectionString}\");");
                }
            }
        }

        protected override void GenerateDbSets(IModel model)
        {
            WriteLine();
            foreach (var entity in model.GetEntityTypes())
            {
                string pluralName = Pluralizer.Pluralize(entity.Name);
                WriteLine($"public virtual DbSet<{entity.Name}> {pluralName} {{ get; set; }}");
            }
        }

        protected override void GenerateConstructors(string contextName)
        {
            WriteLine($"public {contextName}()");
            using (OpenBlock())
            {
            }

            WriteLine($"public {contextName}(DbContextOptions<{contextName}> options) : base(options)");
            using (OpenBlock())
            {
            }
        }

        protected override void GenerateRelationship(IForeignKey foreignKey)
        {
            List<IAnnotation> list = foreignKey.GetAnnotations().ToList();
            string hasOne = "builder.HasOne(" + (foreignKey.DependentToPrincipal != null ? "d => d." + foreignKey.DependentToPrincipal.Name : null) + ")";
            string hasOneOrMany = "." +
                (foreignKey.IsUnique ? "WithOne" : "WithMany") +
                "(" +
                (foreignKey.PrincipalToDependent != null ? "p => p." + foreignKey.PrincipalToDependent.Name : null) +
                ")";

            WriteLine(hasOne);
            WriteLine(hasOneOrMany);

            if (!foreignKey.PrincipalKey.IsPrimaryKey())
            {
                WriteLine(".HasPrincipalKey" +
                               (foreignKey.IsUnique ? "<" + TBE.DisplayName(foreignKey.PrincipalEntityType) + ">" : "") +
                               "(p => " +
                               GenerateLambdaToKey(foreignKey.PrincipalKey.Properties, "p") +
                               ")");
            }

            WriteLine(".HasForeignKey" +
                           (foreignKey.IsUnique ? "<" + TBE.DisplayName(foreignKey.DeclaringEntityType) + ">" : "") +
                           "(d => " +
                           GenerateLambdaToKey(foreignKey.Properties, "d") +
                           ")");
            DeleteBehavior deleteBehavior = foreignKey.IsRequired ? DeleteBehavior.Cascade : DeleteBehavior.ClientSetNull;
            if (foreignKey.DeleteBehavior != deleteBehavior)
            {
                WriteLine(".OnDelete(" + Helper.Literal(foreignKey.DeleteBehavior) + ")");
            }

            if (!string.IsNullOrEmpty((string)foreignKey["Relational:Name"]))
            {
                WriteLine(".HasConstraintName(" + Helper.Literal(foreignKey.Relational().Name) + ")");
                AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:Name");
            }

            List<IAnnotation> annotationList = new List<IAnnotation>();
            foreach (IAnnotation annotation in list)
            {
                if (AnnotationCodeGenerator.IsHandledByConvention(foreignKey, annotation))
                {
                    annotationList.Add(annotation);
                }
                else
                {
                    MethodCallCodeFragment fluentApi = AnnotationCodeGenerator.GenerateFluentApi(foreignKey, annotation);
                    string str = fluentApi == null ?
#pragma warning disable CS0618 // Type or member is obsolete
                        AnnotationCodeGenerator.GenerateFluentApi(foreignKey, annotation, "CSharp") :
#pragma warning restore CS0618 // Type or member is obsolete
                        Helper.Fragment(fluentApi);
                    if (str != null)
                    {
                        Write(str);
                    }
                }
            }

            AnnotationsBuilder.BuildAnnotations(list.Except(annotationList), PushIndent, PopIndent, Write, WriteLine);

            Write(";");
        }

        public static bool IsNullableType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsValueType)
            {
                return true;
            }

            if (typeInfo.IsGenericType)
            {
                return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            return false;
        }

        protected override void GenerateProperty(IEntityType entityType, IProperty property)
        {
            WriteLine();

            List<IAnnotation> list = property.GetAnnotations().ToList();



            if (property.IsPrimaryKey())
            {
                var key = entityType.FindPrimaryKey();
                WriteLine($"builder.HasKey(x=>x.{property.Name})");
                WriteLine($"    .HasName(\"{key.SqlServer().Name}\");");
                WriteLine();
            }

            WriteLine("builder.Property(x => x." + property.Name + ")");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:ColumnName");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:ColumnType");
            AnnotationsBuilder.RemoveAnnotation(ref list, "MaxLength");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:TypeMapping");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Unicode");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:DefaultValue");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:DefaultValueSql");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:ComputedColumnSql");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Relational:IsFixedLength");
            AnnotationsBuilder.RemoveAnnotation(ref list, "Scaffolding:ColumnOrdinal");




            if (!property.IsNullable && IsNullableType(property.ClrType) && !property.IsPrimaryKey())
            {
                WriteLine(".IsRequired()");
            }

            string columnName = property.Relational().ColumnName;
            if (columnName != null && columnName != property.Name)
            {
                WriteLine(".HasColumnName(" + Helper.Literal(columnName) + ")");
            }

            string configuredColumnType = property.SqlServer().ColumnType;
            if (configuredColumnType != null)
            {
                WriteLine(".HasColumnType(" + Helper.Literal(configuredColumnType) + ")");
            }

            int? maxLength = property.GetMaxLength();
            if (maxLength.HasValue)
            {
                WriteLine(".HasMaxLength(" + Helper.Literal(maxLength.Value) + ")");
            }


            bool? nullable = property.IsUnicode();
            if (nullable.HasValue)
            {
                nullable = property.IsUnicode();
                bool flag = false;
                string str = ".IsUnicode(" + ((nullable.GetValueOrDefault() == flag) & nullable.HasValue ? "false" : "") + ")";
                WriteLine(str);
            }

            if (property.Relational().IsFixedLength)
            {
                WriteLine(".IsFixedLength()");
            }

            if (property.Relational().DefaultValue != null)
            {
                WriteLine(".HasDefaultValue(" + Helper.UnknownLiteral(property.Relational().DefaultValue) + ")");
            }

            if (property.Relational().DefaultValueSql != null)
            {
                WriteLine(".HasDefaultValueSql(" + Helper.Literal(property.Relational().DefaultValueSql) + ")");
            }

            if (property.Relational().ComputedColumnSql != null)
            {
                WriteLine(".HasComputedColumnSql(" + Helper.Literal(property.Relational().ComputedColumnSql) + ")");
            }

            ValueGenerated valueGenerated1 = property.ValueGenerated;
            bool flag1 = false;
            if (((Property)property).GetValueGeneratedConfigurationSource().HasValue)
            {
                ValueGenerated? valueGenerated2 = new RelationalValueGeneratorConvention().GetValueGenerated((Property)property);
                ValueGenerated valueGenerated3 = valueGenerated1;
                if (!((valueGenerated2.GetValueOrDefault() == valueGenerated3) & valueGenerated2.HasValue))
                {
                    string str;
                    switch (valueGenerated1)
                    {
                        case ValueGenerated.Never:
                            str = "ValueGeneratedNever";
                            break;
                        case ValueGenerated.OnAdd:
                            str = "ValueGeneratedOnAdd";
                            break;
                        case ValueGenerated.OnAddOrUpdate:
                            flag1 = property.IsConcurrencyToken;
                            str = flag1 ? "IsRowVersion" : "ValueGeneratedOnAddOrUpdate";
                            break;
                        default:
                            str = "";
                            break;
                    }

                    WriteLine("." + str + "()");
                }
            }

            if (property.IsConcurrencyToken && !flag1)
            {
                WriteLine(".IsConcurrencyToken()");
            }

            List<IAnnotation> annotationList = new List<IAnnotation>();
            foreach (IAnnotation annotation in list)
            {
                if (AnnotationCodeGenerator.IsHandledByConvention(property, annotation))
                {
                    annotationList.Add(annotation);
                }
                else
                {
                    MethodCallCodeFragment fluentApi = AnnotationCodeGenerator.GenerateFluentApi(property, annotation);
                    string str = fluentApi == null ?
#pragma warning disable CS0618 // Type or member is obsolete
                        AnnotationCodeGenerator.GenerateFluentApi(property, annotation, "CSharp") :
#pragma warning restore CS0618 // Type or member is obsolete
                        Helper.Fragment(fluentApi);
                    if (str != null)
                    {
                        Write(str);
                        annotationList.Add(annotation);
                    }
                }
            }

            AnnotationsBuilder.BuildAnnotations(list.Except(annotationList), PushIndent, PopIndent, Write, WriteLine);


            WriteLine(";");
        }

        protected override void GenerateIndex(IIndex index)
        {
        }

        private static string GenerateLambdaToKey(
            IReadOnlyList<IProperty> properties,
            string lambdaIdentifier)
        {
            if (properties.Count <= 0)
            {
                return "";
            }

            if (properties.Count != 1)
            {
                return "new { " + string.Join(", ", properties.Select(p => lambdaIdentifier + "." + p.Name)) + " }";
            }

            return lambdaIdentifier + "." + properties[0].Name;
        }
    }
}
