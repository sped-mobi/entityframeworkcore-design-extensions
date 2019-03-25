using JetBrains.Annotations;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Migrations.Design
{
    public abstract class AbstractEfDesignerMigrationOperationGenerator : AbstractCodeGenerator, ICSharpMigrationOperationGenerator
    {
        protected AbstractEfDesignerMigrationOperationGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
        {
        }


        /// <summary>
        ///     Generates code for creating <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.MigrationOperation" /> objects.
        /// </summary>
        /// <param name="builderName"> The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Operations.MigrationOperation" /> variable name. </param>
        /// <param name="operations"> The operations. </param>
        /// <param name="builder"> The builder code is added to. </param>
        public virtual void Generate(
          string builderName,
          IReadOnlyList<MigrationOperation> operations,
          IndentedStringBuilder builder)
        {
            Check.NotEmpty(builderName, nameof(builderName));
            Check.NotNull<IReadOnlyList<MigrationOperation>>(operations, nameof(operations));
            Check.NotNull<IndentedStringBuilder>(builder, nameof(builder));
            bool flag = true;
            foreach (MigrationOperation operation in operations)
            {
                if (flag)
                    flag = false;
                else
                    builder.AppendLine().AppendLine();
                builder.Append(builderName);

                switch (operation)
                {
                    case AddColumnOperation _:
                        {
                            Generate(operation as AddColumnOperation);
                            break;
                        }
                    case AddForeignKeyOperation _:
                        {
                            Generate(operation as AddForeignKeyOperation);
                            break;
                        }
                    case AddPrimaryKeyOperation _:
                        {
                            Generate(operation as AddPrimaryKeyOperation);
                            break;
                        }
                    case AddUniqueConstraintOperation _:
                        {
                            Generate(operation as AddUniqueConstraintOperation);
                            break;
                        }
                    case AlterColumnOperation _:
                        {
                            Generate(operation as AlterColumnOperation);
                            break;
                        }
                    case AlterDatabaseOperation _:
                        {
                            Generate(operation as AlterDatabaseOperation);
                            break;
                        }
                    case AlterSequenceOperation _:
                        {
                            Generate(operation as AlterSequenceOperation);
                            break;
                        }
                    case AlterTableOperation _:
                        {
                            Generate(operation as AlterTableOperation);
                            break;
                        }
                    case ColumnOperation _:
                        {
                            Generate(operation as ColumnOperation);
                            break;
                        }
                    case CreateIndexOperation _:
                        {
                            Generate(operation as CreateIndexOperation);
                            break;
                        }
                    case CreateSequenceOperation _:
                        {
                            Generate(operation as CreateSequenceOperation);
                            break;
                        }
                    case CreateTableOperation _:
                        {
                            Generate(operation as CreateTableOperation);
                            break;
                        }
                    case DeleteDataOperation _:
                        {
                            Generate(operation as DeleteDataOperation);
                            break;
                        }
                    case DropColumnOperation _:
                        {
                            Generate(operation as DropColumnOperation);
                            break;
                        }
                    case DropForeignKeyOperation _:
                        {
                            Generate(operation as DropForeignKeyOperation);
                            break;
                        }
                    case DropIndexOperation _:
                        {
                            Generate(operation as DropIndexOperation);
                            break;
                        }
                    case DropPrimaryKeyOperation _:
                        {
                            Generate(operation as DropPrimaryKeyOperation);
                            break;
                        }
                    case DropSchemaOperation _:
                        {
                            Generate(operation as DropSchemaOperation);
                            break;
                        }
                    case DropSequenceOperation _:
                        {
                            Generate(operation as DropSequenceOperation);
                            break;
                        }
                    case DropTableOperation _:
                        {
                            Generate(operation as DropTableOperation);
                            break;
                        }
                    case DropUniqueConstraintOperation _:
                        {
                            Generate(operation as DropUniqueConstraintOperation);
                            break;
                        }
                    case EnsureSchemaOperation _:
                        {
                            Generate(operation as EnsureSchemaOperation);
                            break;
                        }
                    case InsertDataOperation _:
                        {
                            Generate(operation as InsertDataOperation);
                            break;
                        }
                    case RenameColumnOperation _:
                        {
                            Generate(operation as RenameColumnOperation);
                            break;
                        }
                    case RenameIndexOperation _:
                        {
                            Generate(operation as RenameIndexOperation);
                            break;
                        }
                    case RenameSequenceOperation _:
                        {
                            Generate(operation as RenameSequenceOperation);
                            break;
                        }
                    case RenameTableOperation _:
                        {
                            Generate(operation as RenameTableOperation);
                            break;
                        }
                    case RestartSequenceOperation _:
                        {
                            Generate(operation as RestartSequenceOperation);
                            break;
                        }
                    case SequenceOperation _:
                        {
                            Generate(operation as SequenceOperation);
                            break;
                        }
                    case SqlOperation _:
                        {
                            Generate(operation as SqlOperation);
                            break;
                        }
                    case UpdateDataOperation _:
                        {
                            Generate(operation as UpdateDataOperation);
                            break;
                        }
                }
            }

            builder.Append(";");
        }

        protected abstract void Generate(AddColumnOperation operation);
        protected abstract void Generate(AddForeignKeyOperation operation);
        protected abstract void Generate(AddPrimaryKeyOperation operation);
        protected abstract void Generate(AddUniqueConstraintOperation operation);
        protected abstract void Generate(AlterColumnOperation operation);
        protected abstract void Generate(AlterDatabaseOperation operation);
        protected abstract void Generate(AlterSequenceOperation operation);
        protected abstract void Generate(AlterTableOperation operation);
        protected abstract void Generate(ColumnOperation operation);
        protected abstract void Generate(CreateIndexOperation operation);
        protected abstract void Generate(CreateSequenceOperation operation);
        protected abstract void Generate(CreateTableOperation operation);
        protected abstract void Generate(DeleteDataOperation operation);
        protected abstract void Generate(DropColumnOperation operation);
        protected abstract void Generate(DropForeignKeyOperation operation);
        protected abstract void Generate(DropIndexOperation operation);
        protected abstract void Generate(DropPrimaryKeyOperation operation);
        protected abstract void Generate(DropSchemaOperation operation);
        protected abstract void Generate(DropSequenceOperation operation);
        protected abstract void Generate(DropTableOperation operation);
        protected abstract void Generate(DropUniqueConstraintOperation operation);
        protected abstract void Generate(EnsureSchemaOperation operation);
        protected abstract void Generate(InsertDataOperation operation);
        protected abstract void Generate(RenameColumnOperation operation);
        protected abstract void Generate(RenameIndexOperation operation);
        protected abstract void Generate(RenameSequenceOperation operation);
        protected abstract void Generate(RenameTableOperation operation);
        protected abstract void Generate(RestartSequenceOperation operation);
        protected abstract void Generate(SequenceOperation operation);
        protected abstract void Generate(SqlOperation operation);
        protected abstract void Generate(UpdateDataOperation operation);
    }
}
