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
    public sealed class CSharpModelCodeGenerator : AbstractCSharpModelCodeGenerator
    {
        public CSharpModelCodeGenerator(
            ModelCodeGeneratorDependencies dependencies,
            ICSharpControllerGenerator controllerGenerator,
            ICSharpEntityTypeGenerator entityTypeGenerator,
            ICSharpDbContextGenerator dbContextGenerator) : base(dependencies)
        {
            ControllerGenerator = controllerGenerator;
            EntityTypeGenerator = entityTypeGenerator;
            DbContextGenerator = dbContextGenerator;
        }

        public ICSharpControllerGenerator ControllerGenerator { get; }
        public ICSharpEntityTypeGenerator EntityTypeGenerator { get; }
        public ICSharpDbContextGenerator DbContextGenerator { get; }



        /// <summary>Generates code for a model.</summary>
        /// <param name="model"> The model. </param>
        /// <param name="namespace"> The namespace. </param>
        /// <param name="contextDir"> The directory of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="contextName"> The name of the <see cref="T:Microsoft.EntityFrameworkCore.DbContext" />. </param>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="options"> The options to use during generation. </param>
        /// <returns> The generated model. </returns>
        public override ScaffoldedModel GenerateModel(IModel model,
            string @namespace,
            string contextDir,
            string contextName,
            string connectionString,
            ModelCodeGenerationOptions options)
        {
            var scaffoldedModel = new ScaffoldedModel();
            string str1 = DbContextGenerator.WriteCode(model, @namespace, contextName, connectionString, options.UseDataAnnotations, options.SuppressConnectionStringWarning);
            string path2 = contextName + ".cs";
            scaffoldedModel.ContextFile = new ScaffoldedFile()
            {
                Path = Path.Combine(contextDir, path2),
                Code = str1
            };
            foreach (IEntityType entityType in model.GetEntityTypes())
            {
                string str2 = EntityTypeGenerator.WriteCode(entityType, @namespace, options.UseDataAnnotations);
                string str3 = entityType.DisplayName() + ".cs";
                scaffoldedModel.AdditionalFiles.Add(new ScaffoldedFile()
                {
                    Path = str3,
                    Code = str2
                });
            }
            return scaffoldedModel;
        }


        private static ScaffoldedFile CreateFile(string code, string path)
            => new ScaffoldedFile { Code = code, Path = path };

        /// <summary>
        ///     Gets the programming language supported by this service.
        /// </summary>
        /// <value> The language. </value>
        public override string Language => "C#";
    }
}
