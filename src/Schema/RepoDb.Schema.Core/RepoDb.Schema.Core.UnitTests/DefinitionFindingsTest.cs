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
    /// Contains the tests of the <c>DefinitionFindings</c> class.
    /// </summary>
    [TestClass]
    public class DefinitionFindingsTest
    {
        #region Public Methods

        [TestMethod]
        public void TestDefinitionFindingsStartsEmpty()
        {
            // Act
            var findings = new DefinitionFindings();

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestDefinitionFindingsDescribeWithASchema()
        {
            // Act
            var actual = DefinitionFindings.Describe(new TableInfo("Orders", "sales", "TestDatabase", "TestProvider"));

            // Assert
            Assert.AreEqual("sales.Orders", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void TestDefinitionFindingsDescribeWithoutASchema(string schema)
        {
            // Act
            var actual = DefinitionFindings.Describe(new TableInfo("Orders", schema, "TestDatabase", "TestProvider"));

            // Assert
            Assert.AreEqual("Orders", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefinitionFindingsAddErrorPrefixesTheTable()
        {
            // Setup
            var findings = new DefinitionFindings();
            var table = TableSchemaFactory.CreateTable("Orders", "Id");

            // Act
            findings.AddError(table, "Something is wrong.");

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
            Assert.AreEqual("Table 'dbo.Orders': Something is wrong.", findings.Errors[0], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefinitionFindingsAddWarningPrefixesTheTable()
        {
            // Setup
            var findings = new DefinitionFindings();
            var table = TableSchemaFactory.CreateTable("Orders", "Id");

            // Act
            findings.AddWarning(table, "Something is odd.");

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            Assert.AreEqual("Table 'dbo.Orders': Something is odd.", findings.Warnings[0], StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefinitionFindingsAddWarningWithoutATableDoesNotPrefixTheMessage()
        {
            // Setup
            var findings = new DefinitionFindings();

            // Act
            findings.AddWarning("Two tables are odd.");

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            Assert.AreEqual("Two tables are odd.", findings.Warnings[0], StringComparer.Ordinal);
        }

        #endregion
    }
}
