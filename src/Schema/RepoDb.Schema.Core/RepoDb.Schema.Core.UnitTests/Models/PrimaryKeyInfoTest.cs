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
    public class PrimaryKeyInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestPrimaryKeyInfoNamePropertyDefaultValue()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo(null);
            var actual = primaryKey.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoNameProperty()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo("PK_Person");
            var actual = primaryKey.Name;
            var expected = "PK_Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoColumnsPropertyDefaultValue()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo(null);
            var actual = primaryKey.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoColumnsProperty()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo(null);
            primaryKey.Columns.Add("Id");
            primaryKey.Columns.Add("Name");
            var actual = primaryKey.Columns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        [TestMethod]
        public void TestPrimaryKeyInfoIsClusteredPropertyDefaultValue()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo(null);
            var actual = primaryKey.IsClustered;

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoIsClusteredProperty()
        {
            // Act
            var primaryKey = new PrimaryKeyInfo(null)
            {
                IsClustered = false
            };
            var actual = primaryKey.IsClustered;

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion
    }
}
