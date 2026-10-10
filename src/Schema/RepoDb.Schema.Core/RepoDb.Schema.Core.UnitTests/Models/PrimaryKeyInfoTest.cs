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
    public class PrimaryKeyInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestPrimaryKeyInfoEqualsWithSameValues()
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
        public void TestPrimaryKeyInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((PrimaryKeyInfo)null) == null);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestPrimaryKeyInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "PK_Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoNotEqualsWithDifferentColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Columns = new List<string> { "Key" };

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoNotEqualsWithDifferentIsClustered()
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

        [TestMethod]
        public void TestPrimaryKeyInfoToStringReturnsTheName()
        {
            // Act
            var actual = Create().ToString();

            // Assert
            Assert.AreEqual("PK_Person", actual);
        }

        [TestMethod]
        public void TestPrimaryKeyInfoToStringWithoutANameReturnsNull()
        {
            // Act
            var actual = new PrimaryKeyInfo(null).ToString();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region Helpers

        private static PrimaryKeyInfo Create() =>
            new PrimaryKeyInfo("PK_Person") { Columns = new List<string> { "Id" }, IsClustered = true };

        #endregion
    }
}
