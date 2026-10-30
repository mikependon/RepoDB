#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionValidator</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionValidatorTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionValidatorWithAValidDefinition()
        {
            // Setup
            var definition = CreateValidDefinition();

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorReportsTheStructuralErrors()
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Missing" } });

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "index 'IX_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorReportsTheStrictErrorsWhenTheStructureIsSound()
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = null });

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "no expression", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorDoesNotRunTheStrictChecksWhenTheStructureIsNotSound()
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Missing" } });
            definition.Tables[0].CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = null });

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "index 'IX_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorReportsTheWarningsOfAValidDefinition()
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 3, isNullable: true));
            definition.Tables[0].PrimaryKey = new PrimaryKeyInfo("PK_Child") { Columns = new[] { "Code" } };

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
            Assert.AreEqual(1, result.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorWithADefinitionWithoutTables()
        {
            // Setup
            var definition = new SchemaDefinitionInfo { Tables = null };

            // Act
            var result = SchemaDefinitionValidator.Validate(definition);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            StringAssert.Contains(result.Errors[0], "no tables", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorDescribeWithARootTable()
        {
            // Setup
            var definition = CreateValidDefinition();

            // Act
            var actual = SchemaDefinitionValidator.Describe(definition);

            // Assert
            Assert.AreEqual("Validate definition of dbo.Child", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionValidatorDescribeWithoutARootTable()
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.RootTable = null;

            // Act
            var actual = SchemaDefinitionValidator.Describe(definition);

            // Assert
            Assert.AreEqual("Validate definition", actual, StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a definition of one table that has a primary key, and no problems.
        /// </summary>
        /// <returns>The definition.</returns>
        private static SchemaDefinitionInfo CreateValidDefinition()
        {
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            return TableSchemaFactory.CreateDefinition(table);
        }

        #endregion
    }
}
