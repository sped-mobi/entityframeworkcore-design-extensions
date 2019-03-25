// -----------------------------------------------------------------------
// <copyright file="DataManager.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Design
{
    public class DesignTimeServices : IDesignTimeServices
    {
        /// <summary>
        ///     Configures design-time services. Use this method to override the default design-time services with your
        ///     own implementations.
        /// </summary>
        /// <param name="serviceCollection"> The design-time service collection. </param>
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            DesignTimeService.Initialize(serviceCollection);
        }
    }
}