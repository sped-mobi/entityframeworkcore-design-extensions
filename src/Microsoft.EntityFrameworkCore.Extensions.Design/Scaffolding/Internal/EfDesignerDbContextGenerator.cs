// -----------------------------------------------------------------------
// <copyright file="CSharpDbContextGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using TBE = Microsoft.EntityFrameworkCore.Metadata.Internal.TypeBaseExtensions;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class EfDesignerDbContextGenerator : AbstractEfDesignerDbContextGenerator
    {
        private bool _entityTypeBuilderInitialized;

        public EfDesignerDbContextGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
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

            WriteLine();
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

            WriteLine();
            WriteLine();
            Write($"// Relationship {foreignKey.DeclaringEntityType.Name} --> {foreignKey.PrincipalEntityType.Name}");

            WriteLine();
            Write(hasOne);

            PushIndent();

            WriteLine();
            Write(hasOneOrMany);

            PopIndent();

            if (!foreignKey.PrincipalKey.IsPrimaryKey())
            {
                PushIndent();
                WriteLine();
                Write(".HasPrincipalKey" +
                               (foreignKey.IsUnique ? "<" + TBE.DisplayName(foreignKey.PrincipalEntityType) + ">" : "") +
                               "(p => " +
                               GenerateLambdaToKey(foreignKey.PrincipalKey.Properties, "p") +
                               ")");
                PopIndent();
            }

            PushIndent();
            WriteLine();
            Write(".HasForeignKey" +
                           (foreignKey.IsUnique ? "<" + TBE.DisplayName(foreignKey.DeclaringEntityType) + ">" : "") +
                           "(d => " +
                           GenerateLambdaToKey(foreignKey.Properties, "d") +
                           ")");
            PopIndent();

            DeleteBehavior deleteBehavior = foreignKey.IsRequired ? DeleteBehavior.Cascade : DeleteBehavior.ClientSetNull;
            if (foreignKey.DeleteBehavior != deleteBehavior)
            {
                PushIndent();
                WriteLine();
                Write(".OnDelete(" + Helper.Literal(foreignKey.DeleteBehavior) + ")");
                PopIndent();
            }

            if (!string.IsNullOrEmpty((string)foreignKey["Relational:Name"]))
            {
                PushIndent();
                WriteLine();
                Write(".HasConstraintName(" + Helper.Literal(foreignKey.Relational().Name) + ")");
                RemoveAnnotation(ref list, "Relational:Name");
                PopIndent();
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

            BuildAnnotations(list.Except(annotationList), PushIndent, PopIndent, Write, WriteLine);

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

            List<IAnnotation> list = property.GetAnnotations().ToList();

            if (property.IsPrimaryKey())
            {
                var key = entityType.FindPrimaryKey();
                WriteLine();
                Write($"builder.HasKey(x=>x.{property.Name})");
                PushIndent();
                WriteLine();
                Write($".HasName(\"{key.SqlServer().Name}\");");
                PopIndent();
                WriteLine();
            }

            WriteLine();
            WriteLine();
            Write($"// Property {entityType.Name}.{property.Name}");
            WriteLine();
            Write("builder.Property(x => x." + property.Name + ")");
            RemoveAnnotation(ref list, "Relational:ColumnName");
            RemoveAnnotation(ref list, "Relational:ColumnType");
            RemoveAnnotation(ref list, "MaxLength");
            RemoveAnnotation(ref list, "Relational:TypeMapping");
            RemoveAnnotation(ref list, "Unicode");
            RemoveAnnotation(ref list, "Relational:DefaultValue");
            RemoveAnnotation(ref list, "Relational:DefaultValueSql");
            RemoveAnnotation(ref list, "Relational:ComputedColumnSql");
            RemoveAnnotation(ref list, "Relational:IsFixedLength");
            RemoveAnnotation(ref list, "Scaffolding:ColumnOrdinal");

            if (!property.IsNullable && IsNullableType(property.ClrType) && !property.IsPrimaryKey())
            {
                PushIndent();
                WriteLine();
                Write(".IsRequired()");
                PopIndent();
            }

            string columnName = property.Relational().ColumnName;
            if (columnName != null && columnName != property.Name)
            {
                PushIndent();
                WriteLine();
                Write(".HasColumnName(" + Helper.Literal(columnName) + ")");
                PopIndent();
            }

            string configuredColumnType = property.SqlServer().ColumnType;
            if (configuredColumnType != null)
            {
                PushIndent();
                WriteLine();
                Write(".HasColumnType(" + Helper.Literal(configuredColumnType) + ")");
                PopIndent();
            }

            int? maxLength = property.GetMaxLength();
            if (maxLength.HasValue)
            {
                PushIndent();
                WriteLine();
                Write(".HasMaxLength(" + Helper.Literal(maxLength.Value) + ")");
                PopIndent();
            }

            bool? nullable = property.IsUnicode();
            if (nullable.HasValue)
            {
                nullable = property.IsUnicode();
                bool flag = false;
                string str = ".IsUnicode(" + ((nullable.GetValueOrDefault() == flag) & nullable.HasValue ? "false" : "") + ")";
                PushIndent();
                WriteLine();
                Write(str);
                PopIndent();
            }

            if (property.Relational().IsFixedLength)
            {
                PushIndent();
                WriteLine();
                Write(".IsFixedLength()");
                PopIndent();
            }

            if (property.Relational().DefaultValue != null)
            {
                PushIndent();
                WriteLine();
                Write(".HasDefaultValue(" + Helper.UnknownLiteral(property.Relational().DefaultValue) + ")");
                PopIndent();
            }

            if (property.Relational().DefaultValueSql != null)
            {
                PushIndent();
                WriteLine();
                Write(".HasDefaultValueSql(" + Helper.Literal(property.Relational().DefaultValueSql) + ")");
                PopIndent();
            }

            if (property.Relational().ComputedColumnSql != null)
            {
                PushIndent();
                WriteLine();
                Write(".HasComputedColumnSql(" + Helper.Literal(property.Relational().ComputedColumnSql) + ")");
                PopIndent();
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

                    Write("." + str + "()");
                }
            }

            if (property.IsConcurrencyToken && !flag1)
            {
                PushIndent();
                WriteLine();
                Write(".IsConcurrencyToken()");
                PopIndent();
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

            BuildAnnotations(list.Except(annotationList), PushIndent, PopIndent, Write, WriteLine);

            Write(";");
        }

        protected override void GenerateIndex(IIndex index)
        {
            List<IAnnotation> list = index.GetAnnotations().ToList();

            WriteLine();

            Write("builder.HasIndex(e => " + GenerateLambdaToKey(index.Properties, "e") + ")");
            if (!string.IsNullOrEmpty(index.SqlServer().Name))
            {
                PushIndent();
                WriteLine();
                Write(".HasName(" + Helper.Literal(index.SqlServer().Name) + ")");
                RemoveAnnotation(ref list, "Relational:Name");
                PopIndent();
            }

            if (index.IsUnique)
            {
                PushIndent();
                WriteLine();
                Write(".IsUnique()");
                PopIndent();
            }

            if (index.Relational().Filter != null)
            {
                PushIndent();
                WriteLine();
                Write(".HasFilter(" + Helper.Literal(index.Relational().Filter) + ")");
                RemoveAnnotation(ref list, "Relational:Filter");

            }
            List<IAnnotation> annotationList = new List<IAnnotation>();
            foreach (IAnnotation annotation in list)
            {
                if (AnnotationCodeGenerator.IsHandledByConvention(index, annotation))
                {
                    annotationList.Add(annotation);
                }
                else
                {
                    MethodCallCodeFragment fluentApi = AnnotationCodeGenerator.GenerateFluentApi(index, annotation);
#pragma warning disable CS0618 // Type or member is obsolete
                    string str = fluentApi == null ? AnnotationCodeGenerator.GenerateFluentApi(index, annotation, "CSharp") : Helper.Fragment(fluentApi);
#pragma warning restore CS0618 // Type or member is obsolete
                    if (str != null)
                    {
                        //WriteLine(str);
                        annotationList.Add(annotation);
                    }
                }
            }
            Write(";");
            WriteLine();
            //stringList.AddRange(GenerateAnnotations(list.Except<IAnnotation>(annotationList)));
            //AppendMultiLineFluentApi(index.DeclaringEntityType, stringList);
        }

        private void AppendMultiLineFluentApi(IEntityType entityType, IList<string> lines)
        {
            if (lines.Count <= 0)
                return;
            InitializeEntityTypeBuilder(entityType);
            PushIndent();
            Write("entity" + lines[0]);

            foreach (string str in lines.Skip<string>(1))
            {
                WriteLine();
                Write(str);
            }

            WriteLine(";");
        }


        private void InitializeEntityTypeBuilder(IEntityType entityType)
        {
            if (!_entityTypeBuilderInitialized)
            {
                WriteLine();
                WriteLine("builder.Entity<" + entityType.Name + ">(entity =>");
                Write("{");
            }
            _entityTypeBuilderInitialized = true;
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

        public static void Remove(ref List<IAnnotation> annotations, string annotationName)
        {
            annotations.Remove(annotations.SingleOrDefault(a => a.Name == annotationName));
        }

        public void RemoveAnnotation(ref List<IAnnotation> list, string annotationName)
        {
            list.Remove(list.SingleOrDefault(a => a.Name == annotationName));
        }

        public void BuildAnnotations(
            IEnumerable<IAnnotation> annotations,
            Action pushIndentAction,
            Action popIndentAction,
            Action<string> writeAction,
            Action<string> writeLineAction)
        {
            if (annotations.Count() == 1)
            {
                return;
            }

            List<string> list = annotations.Select(GenerateAnnotation).ToList();

            if (list?.Count > 0)
            {
                pushIndentAction();
                for (int i = 0; i < list.Count; i++)
                {
                    string line = list[i];

                    if (i == list.Count - 1)
                    {
                        writeLineAction(line);
                    }
                    else
                    {
                        writeAction(line);
                    }
                }
                popIndentAction();
            }

        }

        private string GenerateAnnotation(IAnnotation annotation)
        {
            return ".HasAnnotation(" + Helper.Literal(annotation.Name) + ", " + Helper.UnknownLiteral(annotation.Value) + ")";
        }

        private IList<string> GenerateAnnotations(IEnumerable<IAnnotation> annotations)
        {
            return annotations.Select(GenerateAnnotation).ToList();
        }
    }
}