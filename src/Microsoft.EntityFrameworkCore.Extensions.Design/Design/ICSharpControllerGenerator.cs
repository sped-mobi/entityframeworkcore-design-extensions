// -----------------------------------------------------------------------
// <copyright file="ICSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Design
{
    public interface ICSharpControllerGenerator
    {
        string GenerateClass(IEntityType entityType, string @namespace, bool useSwagger);
    }
}
