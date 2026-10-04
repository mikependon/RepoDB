#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
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
        public void TestForeignKeyInfoEqualsWithSameValues()
        {
            // Act
            var a = Create();
            var b = Create();

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestForeignKeyInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestForeignKeyInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((ForeignKeyInfo)null) == null);
        }

        [TestMethod]
        public void TestForeignKeyInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "FK_Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Columns = new List<string> { "OtherId" };

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentReferencedTable()
        {
            // Act
            var a = Create();
            var b = Create();
            b.ReferencedTable = new TableInfo("Other", "dbo");

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentReferencedColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.ReferencedColumns = new List<string> { "OtherKey" };

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentUpdateRule()
        {
            // Act
            var a = Create();
            var b = Create();
            b.UpdateRule = ForeignKeyRule.NoAction;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNotEqualsWithDifferentDeleteRule()
        {
            // Act
            var a = Create();
            var b = Create();
            b.DeleteRule = ForeignKeyRule.NoAction;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestForeignKeyInfoNamePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoNameProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo("FK_Person_Country");
            var actual = foreignKey.Name;
            var expected = "FK_Person_Country";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedTablePropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.ReferencedTable;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedTableProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null)
            {
                ReferencedTable = new TableInfo("Country", "dbo")
            };
            var actual = foreignKey.ReferencedTable;
            var expected = new TableInfo("Country", "dbo");

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoColumnsPropertyDefaultValue()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestForeignKeyInfoColumnsProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null);
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
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.ReferencedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestForeignKeyInfoReferencedColumnsProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null);
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
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.UpdateRule;

            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoUpdateRuleProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null)
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
            var foreignKey = new ForeignKeyInfo(null);
            var actual = foreignKey.DeleteRule;

            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction, actual);
        }

        [TestMethod]
        public void TestForeignKeyInfoDeleteRuleProperty()
        {
            // Act
            var foreignKey = new ForeignKeyInfo(null)
            {
                DeleteRule = ForeignKeyRule.SetNull
            };
            var actual = foreignKey.DeleteRule;
            var expected = ForeignKeyRule.SetNull;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Helpers

        private static ForeignKeyInfo Create() =>
            new ForeignKeyInfo("FK_Person_Country") { Columns = new List<string> { "CountryId" }, ReferencedTable = new TableInfo("Country", "dbo"), ReferencedColumns = new List<string> { "Id" }, UpdateRule = ForeignKeyRule.Cascade, DeleteRule = ForeignKeyRule.SetNull };

        #endregion
    }
}
