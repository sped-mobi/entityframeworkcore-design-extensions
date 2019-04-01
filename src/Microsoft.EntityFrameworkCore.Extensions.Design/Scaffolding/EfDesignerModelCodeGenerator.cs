// -----------------------------------------------------------------------
// <copyright file="AbstractCSharpModelCodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Microsoft.EntityFrameworkCore.Controllers.Design;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace Microsoft.EntityFrameworkCore.Scaffolding
{
    public class EfDesignerModelCodeGenerator : AbstractEfDesignerModelCodeGenerator
    {
        public EfDesignerModelCodeGenerator(
            CodeGeneratorDependencies dependencies,
            ICSharpEntityTypeGenerator entityTypeGenerator,
            ICSharpDbContextGenerator dbContextGenerator,
            IEfDesignerControllerGenerator controllerGenerator) : base(dependencies)
        {
            EntityTypeGenerator = entityTypeGenerator;
            DbContextGenerator = dbContextGenerator;
            ControllerGenerator = controllerGenerator;
        }

        public IEfDesignerControllerGenerator ControllerGenerator { get; }

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
            string dbContextCode = DbContextGenerator.WriteCode(model, @namespace, contextName, connectionString, options.UseDataAnnotations, options.SuppressConnectionStringWarning);
            string path2 = contextName + ".cs";
            scaffoldedModel.ContextFile = new ScaffoldedFile()
            {
                Path = Path.Combine(contextDir, path2),
                Code = dbContextCode
            };
            foreach (IEntityType entityType in model.GetEntityTypes())
            {
                string contextCode = EntityTypeGenerator.WriteCode(entityType, @namespace, options.UseDataAnnotations);
                string fileName = entityType.DisplayName() + ".cs";
                scaffoldedModel.AdditionalFiles.Add(CreateFile(contextCode, fileName));
            }
            foreach (IEntityType entityType in model.GetEntityTypes())
            {
                string controllerCode = ControllerGenerator.WriteCode(entityType, @namespace, contextName);
                string fileName = entityType.DisplayName() + "Controller.cs";
                string path = Path.Combine("Controllers", fileName);

                if (!Directory.Exists("Controllers"))
                    Directory.CreateDirectory("Controllers");

                scaffoldedModel.AdditionalFiles.Add(CreateFile(controllerCode, path));
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
