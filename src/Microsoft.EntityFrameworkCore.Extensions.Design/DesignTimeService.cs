// -----------------------------------------------------------------------
// <copyright file="DesignTimeServices.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public static class DesignTimeService
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {

            serviceCollection.AddSingleton<CodeGeneratorDependencies>();

            serviceCollection.AddSingleton<IStringBuilderProvider, StringBuilderProvider>();
            serviceCollection.AddSingleton<ICSharpHelper, EfDesignerHelper>();
            serviceCollection.AddSingleton<IPluralizer, EfDesignerPluralizer>();

            // Entities
            serviceCollection.AddSingleton<ICSharpEntityTypeGenerator, EfDesignerEntityTypeGenerator>();

            // DbContext
            serviceCollection.AddSingleton<ICSharpDbContextGenerator, EfDesignerDbContextGenerator>();

            // Controllers
            // serviceCollection.AddSingleton<IEfDesignerControllerGenerator, EfDesignerControllerGenerator>();

            // Model
            serviceCollection.AddSingleton<IModelCodeGenerator, EfDesignerModelCodeGenerator>();





            serviceCollection.AddSingleton<IOperationReporter, EfDesignerOperationReporter>();
            serviceCollection.AddSingleton<IOperationReportHandler, EfDesignerOperationReportHandler>();



        }
    }

}
