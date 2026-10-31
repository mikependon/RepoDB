using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class ValidationMessageTest
    {
        [TestMethod]
        [DataRow("Index", "Indexes", "index")]
        [DataRow("Unique", "Unique Constraints", "unique constraint")]
        [DataRow("Primary", "Primary Key", "primary key")]
        public void EmptyColumnSelectionIdentifiesObjectLocationAndRepair(string kind, string collection, string description)
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            if (kind == "Index") table.Indexes.Add(new IndexInfo("New"));
            if (kind == "Unique") table.UniqueConstraints.Add(new UniqueConstraintInfo("New"));
            if (kind == "Primary") table.PrimaryKey = new PrimaryKeyInfo("New");

            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual($"Table 'dbo.Categories': The {description} 'New' has no columns. Select at least one column from the table. Location: Tables > dbo.Categories > {collection} > 'New' > Columns.", result.Errors.Single());
        }

        [TestMethod]
        public void ForeignKeyDistinguishesSourceAndReferencedSelections()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.ForeignKeys.Add(new ForeignKeyInfo("FK_New") { ReferencedTable = table.Table });
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            Assert.AreEqual(2, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "foreign key 'FK_New' has no columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[0], "Foreign Keys > 'FK_New' > Columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "referenced columns of the foreign key 'FK_New' selection is empty", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Select at least one column from the table 'dbo.Categories'", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Foreign Keys > 'FK_New' > Referenced Columns", StringComparison.Ordinal);
        }

        [TestMethod]
        public void UnnamedObjectsHaveOneBasedPositions()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.Indexes.Add(new IndexInfo("Valid") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo(" "));
            table.Indexes.Add(new IndexInfo(" "));
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            StringAssert.Contains(result.Errors[0], "index #2 (unnamed)", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[0], "Indexes > #2 (unnamed) > Columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Indexes > #3 (unnamed) > Columns", StringComparison.Ordinal);
        }

        [TestMethod]
        public void RepeatedUnknownAndIncludedColumnsHaveDistinctGuidance()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.Indexes.Add(new IndexInfo("IX") { Columns = new[] { "Missing", "Missing" }, IncludedColumns = new[] { "Other" } });
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            Assert.AreEqual(3, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "Remove repeated column selections", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Replace these references with existing columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[2], "Indexes > 'IX' > Included Columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[2], "(Other)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void UnnamedCheckConstraintHasExpressionLocationAndRepair()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.CheckConstraints.Add(new CheckConstraintInfo(null));
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            StringAssert.Contains(result.Errors.Single(), "check constraint #1 (unnamed) has no expression. Enter a SQL check expression", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors.Single(), "Check Constraints > #1 (unnamed) > Expression", StringComparison.Ordinal);
        }

        [TestMethod]
        public void InvalidColumnOrdinalsIdentifyColumnsAndRepair()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id", "Name");
            table.Columns[0].Ordinal = 0;
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            StringAssert.Contains(result.Errors.Single(), "'Id' (ordinal 0)", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors.Single(), "Assign each column a distinct ordinal greater than zero", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors.Single(), "Tables > dbo.Categories > Columns", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ForeignKeyCountAndTargetErrorsIdentifyRepairLocations()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.ForeignKeys.Add(new ForeignKeyInfo("FK") { Columns = new[] { "Id" }, ReferencedTable = new TableInfo("Missing", "Sales") });
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            StringAssert.Contains(result.Errors[0], "Select the same number of source and referenced columns, pairing them in order", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[0], "Foreign Keys > 'FK' > Columns / Referenced Columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Sales.Missing", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "declare it in External Dependencies", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Foreign Keys > 'FK' > Referenced Table", StringComparison.Ordinal);
        }

        [TestMethod]
        public void DuplicateIndexNamesHaveRenameGuidance()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.Indexes.Add(new IndexInfo("IX") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo("IX") { Columns = new[] { "Id" } });
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            StringAssert.Contains(result.Errors.Single(), "Give each index a distinct name", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors.Single(), "Tables > dbo.Categories > Indexes > IX > Name", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ExpressionWarningsIdentifyTheExpressionWithoutClaimingToParseSql()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            table.Indexes.Add(new IndexInfo("IX") { Columns = new[] { "Id" }, Filter = "[Missing] > 0" });
            var result = SchemaDefinitionValidator.Validate(TableSchemaFactory.CreateDefinition(table));
            Assert.IsTrue(result.IsValid);
            StringAssert.Contains(result.Warnings.Single(), "Review the identifier against the table columns", StringComparison.Ordinal);
            StringAssert.Contains(result.Warnings.Single(), "The expression is not parsed", StringComparison.Ordinal);
            StringAssert.Contains(result.Warnings.Single(), "Indexes > 'IX' > Filter", StringComparison.Ordinal);
        }

        [TestMethod]
        public void RootAndExternalDependencyMessagesIdentifyConflictingTables()
        {
            var table = TableSchemaFactory.CreateTable("Categories", "Id");
            var definition = TableSchemaFactory.CreateDefinition(table);
            definition.RootTable = new TableInfo("Missing", "Sales");
            definition.ExternalDependencies = new[] { table.Table };
            var result = SchemaDefinitionValidator.Validate(definition);
            Assert.AreEqual(2, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "Location: Root Table (Sales.Missing)", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[0], "Select a Root Table that exists in Tables", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Conflicting tables: dbo.Categories", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "Location: External Dependencies", StringComparison.Ordinal);
        }
    }
}
