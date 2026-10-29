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
    /// Contains the tests of the <c>SchemaDefinitionStrictValidator</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionStrictValidatorTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionStrictValidatorWithAValidDefinition()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentId" }, new[] { "Id" });
            var definition = new SchemaDefinitionResult { Tables = new List<TableSchema> { parent, child } };

            // Act
            var findings = SchemaDefinitionStrictValidator.Validate(definition);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionStrictValidatorCollectsTheProblemsOfAllTheTables()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            parent.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Id" } });
            parent.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Code" } });
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentCode");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentCode" }, new[] { "Code" });
            var empty = TableSchemaFactory.CreateTable("Empty");
            var definition = new SchemaDefinitionResult { Tables = new List<TableSchema> { parent, child, empty } };

            // Act
            var findings = SchemaDefinitionStrictValidator.Validate(definition);

            // Assert
            Assert.AreEqual(3, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Table 'dbo.Parent'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[1], "Table 'dbo.Child'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[2], "Table 'dbo.Empty'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionStrictValidatorSeparatesTheErrorsFromTheWarnings()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 1, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Code");
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = null });
            var definition = new SchemaDefinitionResult { Tables = new List<TableSchema> { table } };

            // Act
            var findings = SchemaDefinitionStrictValidator.Validate(definition);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Errors[0], "no expression", StringComparison.Ordinal);
            StringAssert.Contains(findings.Warnings[0], "is nullable", StringComparison.Ordinal);
        }

        #endregion
    }
}
