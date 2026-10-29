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
    /// Contains the tests of the <c>SchemaDefinitionExpressionRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionExpressionRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionExpressionRulesWithExpressionsOfTheColumnsOfTheTable()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Age", "Total", "Name");
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Age") { Expression = "([Age] > 0 AND [AGE] < 150)" });
            table.Columns[1].ComputedExpression = "([Age] * 2)";
            table.Indexes.Add(new IndexInfo("IX_Name") { Columns = new[] { "Name" }, Filter = "[Name] IS NOT NULL" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void TestSchemaDefinitionExpressionRulesWithACheckConstraintWithoutExpression(string expression)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Age");
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Age") { Expression = expression });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "check constraint 'CK_Age' has no expression", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionRulesWithACheckConstraintThatRefersToAnUnknownColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Age");
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Age") { Expression = "[Age] > [Missing]" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "check constraint 'CK_Age' refers to 'Missing'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionRulesWithAComputedColumnThatRefersToAnUnknownColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Price", "Total");
            table.Columns[1].ComputedExpression = "([Price] * [Quantity])";
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "computed column 'Total' refers to 'Quantity'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionRulesWithAnIndexFilterThatRefersToAnUnknownColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Name");
            table.Indexes.Add(new IndexInfo("IX_Name") { Columns = new[] { "Name" }, Filter = "[Deleted] = 0" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "filter of the index 'IX_Name' refers to 'Deleted'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionRulesIgnoresTheLiteralsTheFunctionsAndTheQualifiedNames()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Name");
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Name") { Expression = "[Name] <> '[Reserved]' AND [dbo].[IsValid]([Name]) = 1" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExpressionRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        #endregion
    }
}
