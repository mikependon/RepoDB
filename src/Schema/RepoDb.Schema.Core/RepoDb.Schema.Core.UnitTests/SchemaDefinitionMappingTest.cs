using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaDefinitionMappingTest
    {
        [TestMethod]
        public void PrefixMapsCrossSchemaAndSelfReferencesWithoutChangingSource()
        {
            var source = new TableSchema("Order", "Sales");
            source.ForeignKeys.Add(new ForeignKeyInfo("FK_Customer")
            {
                Columns = new[] { "CustomerId" }, ReferencedColumns = new[] { "Id" },
                ReferencedTable = new TableInfo("Customer", "HR"), DeleteRule = ForeignKeyRule.Cascade
            });
            source.ForeignKeys.Add(new ForeignKeyInfo("FK_Self") { ReferencedTable = source.Table });
            var target = SchemaDefinitionMapping.Map(source, SchemaMappingMode.Prefix);
            Assert.AreEqual("Sales_Order", target.Table.Name);
            Assert.IsNull(target.Table.Schema);
            Assert.AreEqual("HR_Customer", target.ForeignKeys[0].ReferencedTable.Name);
            Assert.AreEqual("Sales_Order", target.ForeignKeys[1].ReferencedTable.Name);
            Assert.AreEqual(ForeignKeyRule.Cascade, target.ForeignKeys[0].DeleteRule);
            Assert.AreSame(source.Columns, target.Columns);
            Assert.AreSame(source.Indexes, target.Indexes);
            Assert.AreSame(source.PrimaryKey, target.PrimaryKey);
            Assert.AreEqual("Sales", source.Table.Schema);
            Assert.AreEqual("Customer", source.ForeignKeys[0].ReferencedTable.Name);
            Assert.AreEqual("Sales_Order", SchemaDefinitionMapping.Map(target, SchemaMappingMode.Prefix).Table.Name);
        }

        [TestMethod]
        public void PreserveRequiresSchemaSupportButAllowsUnqualifiedTables()
        {
            var definition = Definition(new TableSchema("Customer", "Sales"));
            var setting = new Mock<IDbSetting>();
            Assert.ThrowsExactly<NotSupportedException>(() => SchemaDefinitionMapping.Validate(definition, setting.Object, SchemaMappingMode.Preserve));
            setting.SetupGet(value => value.IsSchemaSupported).Returns(true);
            SchemaDefinitionMapping.Validate(definition, setting.Object, SchemaMappingMode.Preserve);
            definition.Tables[0].Table.Schema = null;
            SchemaDefinitionMapping.Validate(definition, null, SchemaMappingMode.Preserve);
        }

        [TestMethod]
        public void IgnoreRejectsDuplicateNamesIncludingExternalReferences()
        {
            var definition = Definition(new TableSchema("Customer", "Sales"), new TableSchema("Customer", "HR"));
            Assert.ThrowsExactly<InvalidOperationException>(() => SchemaDefinitionMapping.Validate(definition, null, SchemaMappingMode.Ignore));
            SchemaDefinitionMapping.Validate(definition, null, SchemaMappingMode.Prefix);
            definition.Tables.RemoveAt(1);
            definition.ExternalDependencies = new[] { new TableInfo("customer", "HR") };
            Assert.ThrowsExactly<InvalidOperationException>(() => SchemaDefinitionMapping.Validate(definition, null, SchemaMappingMode.Ignore));
        }

        [TestMethod]
        public void PrefixRejectsAmbiguousFlattenedNames()
        {
            var definition = Definition(new TableSchema("Order_Item", "Sales"), new TableSchema("Item", "Sales_Order"));
            Assert.ThrowsExactly<InvalidOperationException>(() => SchemaDefinitionMapping.Validate(definition, null, SchemaMappingMode.Prefix));
        }

        [TestMethod]
        public void IgnoreClearsSchemaAndPreservesNamesAndColumnPairs()
        {
            var source = new TableSchema("Order", "Sales");
            source.ForeignKeys.Add(new ForeignKeyInfo("FK") { ReferencedTable = new TableInfo("Customer", "HR"), Columns = new[] { "CustomerId" }, ReferencedColumns = new[] { "Id" } });
            var target = SchemaDefinitionMapping.Map(source, SchemaMappingMode.Ignore);
            Assert.AreEqual("Order", target.Table.Name);
            Assert.IsNull(target.Table.Schema);
            Assert.IsNull(target.ForeignKeys[0].ReferencedTable.Schema);
            CollectionAssert.AreEqual(new[] { "CustomerId" }, new List<string>(target.ForeignKeys[0].Columns));
            CollectionAssert.AreEqual(new[] { "Id" }, new List<string>(target.ForeignKeys[0].ReferencedColumns));
        }

        private static SchemaDefinitionInfo Definition(params TableSchema[] tables) => new SchemaDefinitionInfo { Tables = new List<TableSchema>(tables) };
    }
}
