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
    /// Contains the tests of the <c>SchemaDefinitionColumnRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionColumnRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionColumnRulesWithValidColumns()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionColumnRulesWithATableWithoutColumns()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Empty");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "has no columns", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionColumnRulesWithANullablePrimaryKeyColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Name");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 2, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Name", "Code");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "primary key column 'Code' is nullable", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionColumnRulesWithANullableColumnThatIsNotInThePrimaryKey()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Name", typeof(string), 2, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        [DataRow(typeof(byte))]
        [DataRow(typeof(sbyte))]
        [DataRow(typeof(short))]
        [DataRow(typeof(ushort))]
        [DataRow(typeof(int))]
        [DataRow(typeof(uint))]
        [DataRow(typeof(long))]
        [DataRow(typeof(ulong))]
        [DataRow(typeof(decimal))]
        [DataRow(typeof(int?))]
        [DataRow(null)]
        public void TestSchemaDefinitionColumnRulesWithAnIdentityOfAnAllowedType(Type type)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Id", type, 1, isIdentity: true));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        [DataRow(typeof(string))]
        [DataRow(typeof(Guid))]
        [DataRow(typeof(double))]
        [DataRow(typeof(DateTime))]
        [DataRow(typeof(bool))]
        public void TestSchemaDefinitionColumnRulesWithAnIdentityOfAnotherType(Type type)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Id", type, 1, isIdentity: true));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "identity column 'Id' is not of a numeric type", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionColumnRulesWithMoreThanOneIdentityColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Id", typeof(int), 1, isIdentity: true));
            table.Columns.Add(TableSchemaFactory.CreateColumn("Number", typeof(long), 2, isIdentity: true));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionColumnRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "more than one identity column", StringComparison.Ordinal);
        }

        #endregion
    }
}
