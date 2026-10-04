#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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
    }
}
