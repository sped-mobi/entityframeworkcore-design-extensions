// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpModelCodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Microsoft.EntityFrameworkCore.Design.Model
{
    public abstract class AbstractCSharpModelCodeGenerator : ModelCodeGenerator, IModelCodeGenerator
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Microsoft.EntityFrameworkCore.Scaffolding.ModelCodeGenerator" /> class.
        /// </summary>
        /// <param name="dependencies"> The dependencies. </param>
        protected AbstractCSharpModelCodeGenerator(ModelCodeGeneratorDependencies dependencies) : base(dependencies)
        {
        }
    }
}
