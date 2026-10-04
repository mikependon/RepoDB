#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class IndexInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestIndexInfoEqualsWithSameValues()
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
        public void TestIndexInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestIndexInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((IndexInfo)null) == null);
        }

        [TestMethod]
        public void TestIndexInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "IX_Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentIsUnique()
        {
            // Act
            var a = Create();
            var b = Create();
            b.IsUnique = false;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Columns = new List<string> { "Age" };

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentIncludedColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.IncludedColumns = new List<string>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentDescendingColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.DescendingColumns = new List<string>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentIsClustered()
        {
            // Act
            var a = Create();
            var b = Create();
            b.IsClustered = false;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNotEqualsWithDifferentFilter()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Filter = null;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestIndexInfoNamePropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIndexInfoNameProperty()
        {
            // Act
            var index = new IndexInfo("IX_Person_Name");
            var actual = index.Name;
            var expected = "IX_Person_Name";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestIndexInfoIsUniquePropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.IsUnique;

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestIndexInfoIsUniqueProperty()
        {
            // Act
            var index = new IndexInfo(null)
            {
                IsUnique = true
            };
            var actual = index.IsUnique;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIndexInfoColumnsPropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestIndexInfoColumnsProperty()
        {
            // Act
            var index = new IndexInfo(null);
            index.Columns.Add("Id");
            index.Columns.Add("Name");
            var actual = index.Columns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void TestIndexInfoIncludedColumnsPropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.IncludedColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestIndexInfoIncludedColumnsProperty()
        {
            // Act
            var index = new IndexInfo(null);
            index.IncludedColumns.Add("Id");
            index.IncludedColumns.Add("Name");
            var actual = index.IncludedColumns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void TestIndexInfoIsClusteredPropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.IsClustered;

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestIndexInfoIsClusteredProperty()
        {
            // Act
            var index = new IndexInfo(null)
            {
                IsClustered = true
            };
            var actual = index.IsClustered;

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestIndexInfoFilterPropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.Filter;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestIndexInfoFilterProperty()
        {
            // Act
            var index = new IndexInfo(null)
            {
                Filter = "([IsActive]=(1))"
            };
            var actual = index.Filter;

            // Assert
            Assert.AreEqual("([IsActive]=(1))", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestIndexInfoDescendingColumnsPropertyDefaultValue()
        {
            // Act
            var index = new IndexInfo(null);
            var actual = index.DescendingColumns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestIndexInfoDescendingColumnsProperty()
        {
            // Act
            var index = new IndexInfo(null);
            index.DescendingColumns.Add("Price");
            var actual = index.DescendingColumns;

            // Assert
            CollectionAssert.AreEqual(new[] { "Price" }, actual.ToArray());
        }

        #endregion

        #region Helpers

        private static IndexInfo Create() =>
            new IndexInfo("IX_Person_Name") { IsUnique = true, Columns = new List<string> { "Name" }, IncludedColumns = new List<string> { "Age" }, DescendingColumns = new List<string> { "Name" }, IsClustered = true, Filter = "[Age] > 0" };

        #endregion
    }
}
