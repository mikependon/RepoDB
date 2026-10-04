#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class ForeignKeyInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestForeignKeyInfoNamePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoNameProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo
            {
                Name = "FK_Person_Country"
            };
            var actual = foreignKey.Name;
            var expected = "FK_Person_Country";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedTablePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.ReferencedTable;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedTableProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo
            {
                ReferencedTable = "dbo.Country"
            };
            var actual = foreignKey.ReferencedTable;
            var expected = "dbo.Country";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestForeignKeyInfoColumnsPropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestForeignKeyInfoColumnsProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            foreignKey.Columns.Add("Id");
            foreignKey.Columns.Add("Name");
            var actual = foreignKey.Columns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedColumnsPropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.ReferencedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedColumnsProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            foreignKey.ReferencedColumns.Add("Id");
            foreignKey.ReferencedColumns.Add("Name");
            var actual = foreignKey.ReferencedColumns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void TestForeignKeyInfoUpdateRulePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.UpdateRule;

            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoUpdateRuleProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo
            {
                UpdateRule = ForeignKeyRule.Cascade
            };
            var actual = foreignKey.UpdateRule;
            var expected = ForeignKeyRule.Cascade;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoDeleteRulePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo();
            var actual = foreignKey.DeleteRule;

            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoDeleteRuleProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo
            {
                DeleteRule = ForeignKeyRule.SetNull
            };
            var actual = foreignKey.DeleteRule;
            var expected = ForeignKeyRule.SetNull;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
