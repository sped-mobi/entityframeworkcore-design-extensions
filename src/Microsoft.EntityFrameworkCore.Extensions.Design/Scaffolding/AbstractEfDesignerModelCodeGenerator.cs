// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpModelCodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Microsoft.EntityFrameworkCore.Scaffolding
{
    /// <summary>Used to generate code for a model.</summary>
    public abstract class AbstractEfDesignerModelCodeGenerator : AbstractCodeGenerator, IModelCodeGenerator
    {
        protected AbstractEfDesignerModelCodeGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
        {
        }


        /// <summary>
        ///     Gets the programming language supported by this service.
        /// </summary>
        /// <value> The language. </value>
        public abstract string Language { get; }



        /// <summary>Generates code for a model.</summary>
        /// <param name="model"> The model. </param>
        /// <param name="namespace"> The namespace. </param>
        /// <param name="contextDir"> The directory of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="contextName"> The name of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="options"> The options to use during generation. </param>
        /// <returns> The generated model. </returns>
        public abstract ScaffoldedModel GenerateModel(
          IModel model,
          string @namespace,
          string contextDir,
          string contextName,
          string connectionString,
          ModelCodeGenerationOptions options);
    }
}
