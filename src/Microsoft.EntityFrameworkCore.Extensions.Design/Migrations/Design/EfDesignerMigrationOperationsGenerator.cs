using JetBrains.Annotations;
using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.EntityFrameworkCore.Migrations.Design
{
    public class EfDesignerMigrationOperationsGenerator : AbstractEfDesignerMigrationOperationGenerator
    {
        public EfDesignerMigrationOperationsGenerator(CodeGeneratorDependencies depenencies) : base(depenencies)
        {
        }

        protected override void Generate(AddColumnOperation operation)
        {
            Write(".AddColumn(");
            Write(")");
            Write(";");
        }

        protected override void Generate(AddForeignKeyOperation operation)
        {
        }

        protected override void Generate(AddPrimaryKeyOperation operation)
        {
        }

        protected override void Generate(AddUniqueConstraintOperation operation)
        {
        }

        protected override void Generate(AlterColumnOperation operation)
        {
        }

        protected override void Generate(AlterDatabaseOperation operation)
        {
        }

        protected override void Generate(AlterSequenceOperation operation)
        {
        }

        protected override void Generate(AlterTableOperation operation)
        {
        }

        protected override void Generate(ColumnOperation operation)
        {
        }

        protected override void Generate(CreateIndexOperation operation)
        {
        }

        protected override void Generate(CreateSequenceOperation operation)
        {
        }

        protected override void Generate(CreateTableOperation operation)
        {
        }

        protected override void Generate(DeleteDataOperation operation)
        {
        }

        protected override void Generate(DropColumnOperation operation)
        {
        }

        protected override void Generate(DropForeignKeyOperation operation)
        {
        }

        protected override void Generate(DropIndexOperation operation)
        {
        }

        protected override void Generate(DropPrimaryKeyOperation operation)
        {
        }

        protected override void Generate(DropSchemaOperation operation)
        {
        }

        protected override void Generate(DropSequenceOperation operation)
        {
        }

        protected override void Generate(DropTableOperation operation)
        {
        }

        protected override void Generate(DropUniqueConstraintOperation operation)
        {
        }

        protected override void Generate(EnsureSchemaOperation operation)
        {
        }

        protected override void Generate(InsertDataOperation operation)
        {
        }

        protected override void Generate(RenameColumnOperation operation)
        {
        }

        protected override void Generate(RenameIndexOperation operation)
        {
        }

        protected override void Generate(RenameSequenceOperation operation)
        {
        }

        protected override void Generate(RenameTableOperation operation)
        {
        }

        protected override void Generate(RestartSequenceOperation operation)
        {
        }

        protected override void Generate(SequenceOperation operation)
        {
        }

        protected override void Generate(SqlOperation operation)
        {
        }

        protected override void Generate(UpdateDataOperation operation)
        {
        }
    }

}
