#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionForeignKeyRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionForeignKeyRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToThePrimaryKey()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToAUniqueConstraint()
        {
            // Setup
            var parent = CreateParent();
            parent.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Parent") { Columns = new[] { "Code" } });
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Code" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToAUniqueIndex()
        {
            // Setup
            var parent = CreateParent();
            parent.Indexes.Add(new IndexInfo("UX_Parent") { Columns = new[] { "Code" }, IsUnique = true });
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Code" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithACompositeKeyInAnotherOrder()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "A", "B");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "A", "B");
            var child = TableSchemaFactory.CreateTable("Child", "X", "Y");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "Y", "X" }, new[] { "B", "A" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToColumnsThatAreNotAKey()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Code" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "foreign key 'FK_Child' references the columns (Code)", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "not its primary key or a unique key", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToOnlyPartOfACompositeKey()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "A", "B");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "A", "B");
            var child = TableSchemaFactory.CreateTable("Child", "X");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "X" }, new[] { "A" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToAFilteredUniqueIndex()
        {
            // Setup
            var parent = CreateParent();
            parent.Indexes.Add(new IndexInfo("UX_Parent") { Columns = new[] { "Code" }, IsUnique = true, Filter = "[Code] IS NOT NULL" });
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Code" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAReferenceToANonUniqueIndex()
        {
            // Setup
            var parent = CreateParent();
            parent.Indexes.Add(new IndexInfo("IX_Parent") { Columns = new[] { "Code" } });
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Code" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithASelfReference()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Node", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Node", "Id");
            TableSchemaFactory.AddForeignKey(table, "FK_Node", table.Table, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(table, new Dictionary<TableInfo, TableSchema> { { table.Table, table } }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAParentThatIsNotPartOfTheDefinition()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, new Dictionary<TableInfo, TableSchema> { { child.Table, child } }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "cannot be verified", StringComparison.Ordinal);
            StringAssert.Contains(findings.Warnings[0], "'dbo.Parent'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAForeignKeyWithoutAReferencedTable()
        {
            // Setup
            var child = TableSchemaFactory.CreateTable("Child", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", null, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, new Dictionary<TableInfo, TableSchema> { { child.Table, child } }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithColumnsOfDifferentTypes()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Id" });
            child.Columns[1] = TableSchemaFactory.CreateColumn("ParentId", typeof(long), 2);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "'ParentId' of the foreign key 'FK_Child' is a Int64", StringComparison.Ordinal);
            StringAssert.Contains(findings.Warnings[0], "'Id' that it references is a Int32", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithTheSameTypeThatIsNullableOnOneSide()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Id" });
            child.Columns[1] = TableSchemaFactory.CreateColumn("ParentId", typeof(int?), 2, isNullable: true);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionForeignKeyRulesWithAnUnknownType()
        {
            // Setup
            var parent = CreateParent();
            var child = CreateChild(parent, new[] { "ParentId" }, new[] { "Id" });
            child.Columns[1] = TableSchemaFactory.CreateColumn("ParentId", null, 2);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionForeignKeyRules.Check(child, Index(parent, child), findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a table that has the given columns and the primary key.
        /// </summary>
        /// <returns>The referenced table, whose primary key is the column Id.</returns>
        private static TableSchema CreateParent()
        {
            var parent = TableSchemaFactory.CreateTable("Parent", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            return parent;
        }

        /// <summary>
        /// Creates a table that has a foreign key to the parent.
        /// </summary>
        /// <param name="parent">The referenced table.</param>
        /// <param name="columns">The columns of the foreign key.</param>
        /// <param name="referencedColumns">The referenced columns.</param>
        /// <returns>The table that has the foreign key.</returns>
        private static TableSchema CreateChild(TableSchema parent,
            string[] columns,
            string[] referencedColumns)
        {
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, columns, referencedColumns);
            return child;
        }

        /// <summary>
        /// Indexes the tables by their identity, as the strict validator does.
        /// </summary>
        /// <param name="tables">The tables of the definition.</param>
        /// <returns>The tables by their identity.</returns>
        private static IDictionary<TableInfo, TableSchema> Index(params TableSchema[] tables)
        {
            var index = new Dictionary<TableInfo, TableSchema>();
            foreach (var table in tables)
            {
                index.Add(table.Table, table);
            }
            return index;
        }

        #endregion
    }
}
