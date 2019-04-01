// -----------------------------------------------------------------------
// <copyright file="ICSharpControllerGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Controllers.Design
{
    public interface IEfDesignerControllerGenerator
    {
        string WriteCode(IEntityType entityType, string @namespace, string contextName);
    }
}
