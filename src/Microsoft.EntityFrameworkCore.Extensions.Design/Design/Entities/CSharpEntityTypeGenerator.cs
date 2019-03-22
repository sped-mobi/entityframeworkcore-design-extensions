// -----------------------------------------------------------------------
// <copyright file="CSharpEntityTypeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Design.Entities
{
    public class CSharpEntityTypeGenerator : AbstractCSharpEntityTypeGenerator
    {
        public CSharpEntityTypeGenerator(IDbContextServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override void GenerateClass(IEntityType entityType)
        {
            WriteLine();
            if (UseDataAnnotations)
            {
                GenerateTypeDataAnnotations(entityType);
            }
            WriteLine($"public partial class {entityType.Name} : I{entityType.Name}");
            using (OpenBlock())
            {
                GenerateConstructor(entityType);
                GenerateProperties(entityType);
                GenerateNavigationProperties(entityType);
            }
        }

        private void GenerateTypeDataAnnotations(IEntityType entityType)
        {
        }

        private void GenerateNavigationProperties(IEntityType entityType)
        {
            IOrderedEnumerable<INavigation> source = entityType.GetNavigations().OrderBy(n => !n.IsDependentToPrincipal() ? 1 : 0).ThenBy(n => !n.IsCollection() ? 0 : 1);
            if (!source.Any())
                return;
            WriteLine();
            foreach (INavigation navigation in source)
            {
                if (UseDataAnnotations)
                    GenerateNavigationDataAnnotations(navigation);
                string name = navigation.GetTargetType().Name;
                WriteLine("public " + (navigation.IsCollection() ? "ICollection<" + name + ">" : name) + " " + navigation.Name + " { get; set; }");
            }
        }

        private void GenerateNavigationDataAnnotations(INavigation navigation)
        {
        }

        private void GenerateProperties(IEntityType entityType)
        {
            foreach (IProperty property in entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal))
            {
                if (UseDataAnnotations)
                {
                    GeneratePropertyDataAnnotations(property);
                }

                WriteLine();

                if (property.IsShadowProperty)
                {
                    //WriteLine("// Shadow Property");
                }

                WriteLine("public " + Helper.Reference(property.ClrType) + " " + property.Name + " { get; set; }");
            }
        }

        private void GeneratePropertyDataAnnotations(IProperty property)
        {

        }

        private void GenerateConstructor(IEntityType entityType)
        {
            foreach (IProperty property in entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal))
            {
                if (UseDataAnnotations)
                {
                    GeneratePropertyDataAnnotations(property);
                }

                WriteLine();

                WriteLine("public " + Helper.Reference(property.ClrType) + " " + property.Name + " { get; set; }");
            }
        }

        protected override void GenerateInterface(IEntityType entityType)
        {
            WriteLine($"public partial interface I{entityType.Name}");
            using (OpenBlock())
            {
                GenerateInterfaceProperties(entityType);
                GenerateInterfaceNavigationProperties(entityType);
            }
        }

        private void GenerateInterfaceNavigationProperties(IEntityType entityType)
        {
            IOrderedEnumerable<INavigation> source = entityType.GetNavigations().OrderBy(n => !n.IsDependentToPrincipal() ? 1 : 0).ThenBy(n => !n.IsCollection() ? 0 : 1);
            if (!source.Any())
                return;
            WriteLine();
            foreach (INavigation navigation in source)
            {
                string navigationName = navigation.Name;
                string targetTypeName = navigation.GetTargetType().Name;
                WriteLine((navigation.IsCollection() ? "ICollection<" + targetTypeName + ">" : targetTypeName) + " " + navigationName + " { get; set; }");
            }
        }

        private void GenerateInterfaceProperties(IEntityType entityType)
        {
            foreach (IProperty property in entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal))
            {
                WriteLine(Helper.Reference(property.ClrType) + " " + property.Name + " { get; set; }");
            }
        }
    }
}
