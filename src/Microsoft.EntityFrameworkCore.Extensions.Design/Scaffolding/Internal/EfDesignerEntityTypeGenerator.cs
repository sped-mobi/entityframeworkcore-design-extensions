// -----------------------------------------------------------------------
// <copyright file="CSharpEntityTypeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class EfDesignerEntityTypeGenerator : AbstractEfDesignerEntityTypeGenerator
    {
        public EfDesignerEntityTypeGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
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
            WriteLine();
            WriteLine($"public {entityType.Name}()");
            using (OpenBlock())
            {
                IOrderedEnumerable<INavigation> source = entityType.GetNavigations().OrderBy(n => !n.IsDependentToPrincipal() ? 1 : 0).ThenBy(n => !n.IsCollection() ? 0 : 1);
                if (!source.Any())
                    return;
                foreach (INavigation navigation in source)
                {
                    string navigationName = navigation.Name;
                    string targetTypeName = navigation.GetTargetType().Name;
                    string typeName = "";
                    if (navigation.IsCollection())
                        typeName = "HashSet<" + targetTypeName + ">();";
                    else
                        typeName = targetTypeName + "();";

                    WriteLine($"{navigationName} = new {typeName}");
                }
            }
        }

        private void GenerateViewModelConstructor(IEntityType entityType)
        {
            WriteLine();
            WriteLine($"public {entityType.Name}ViewModel()");
            using (OpenBlock())
            {
                IOrderedEnumerable<INavigation> source = entityType.GetNavigations().OrderBy(n => !n.IsDependentToPrincipal() ? 1 : 0).ThenBy(n => !n.IsCollection() ? 0 : 1);
                if (!source.Any())
                    return;
                foreach (INavigation navigation in source)
                {
                    string navigationName = navigation.Name;
                    string targetTypeName = navigation.GetTargetType().Name + "ViewModel";
                    string typeName = "";
                    if (navigation.IsCollection())
                        typeName = "HashSet<" + targetTypeName + ">();";
                    else
                        typeName = targetTypeName + "();";

                    WriteLine($"{navigationName} = new {typeName}");
                }
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

        private void GenerateViewModelProperties(IEntityType entityType)
        {
            var skipProperties = entityType.GetKeys().SelectMany(k => k.Properties).ToList();


            foreach (IProperty property in entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal))
            {
                if (property.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) &&
                    property.ClrType == typeof(Guid))
                {
                    continue;
                }

                WriteLine("public " + Helper.Reference(property.ClrType) + " " + property.Name + " { get; set; }");
            }
        }

        private void GenerateViewModelNavigationProperties(IEntityType entityType)
        {
            IOrderedEnumerable<INavigation> source = entityType.GetNavigations().OrderBy(n => !n.IsDependentToPrincipal() ? 1 : 0).ThenBy(n => !n.IsCollection() ? 0 : 1);
            if (!source.Any())
                return;
            WriteLine();
            foreach (INavigation navigation in source)
            {
                if (UseDataAnnotations)
                    GenerateNavigationDataAnnotations(navigation);
                string name = navigation.GetTargetType().Name + "ViewModel";
                WriteLine("public " + (navigation.IsCollection() ? "ICollection<" + name + ">" : name) + " " + navigation.Name + " { get; set; }");
            }
        }

        protected override void GenerateViewModel(IEntityType entityType)
        {
            WriteLine($"public partial class {entityType.Name}ViewModel");
            using (OpenBlock())
            {
                GenerateViewModelConstructor(entityType);
                GenerateViewModelProperties(entityType);
                GenerateViewModelNavigationProperties(entityType);
            }
        }
    }
}
