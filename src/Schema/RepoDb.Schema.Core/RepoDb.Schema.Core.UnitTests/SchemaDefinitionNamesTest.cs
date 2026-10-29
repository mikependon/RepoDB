#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionNames</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionNamesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionNamesWithATableWithoutConstraints()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");

            // Act
            var actual = SchemaDefinitionNames.GetConstraintNames(table);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionNamesGetsTheConstraintsInOrder()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Child") { Columns = new[] { "Id" } });
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = "[Id] > 0" });
            TableSchemaFactory.AddForeignKey(table, "FK_Child", new TableInfo("Parent", "dbo", "TestDatabase", "TestProvider"), new[] { "ParentId" }, new[] { "Id" });

            // Act
            var actual = SchemaDefinitionNames.GetConstraintNames(table);

            // Assert
            CollectionAssert.AreEqual(new[] { "PK_Child", "UQ_Child", "CK_Child", "FK_Child" }, new System.Collections.Generic.List<string>(actual));
        }

        [TestMethod]
        public void TestSchemaDefinitionNamesDoesNotGetTheIndexes()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" } });

            // Act
            var actual = SchemaDefinitionNames.GetConstraintNames(table);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        #endregion
    }
}
