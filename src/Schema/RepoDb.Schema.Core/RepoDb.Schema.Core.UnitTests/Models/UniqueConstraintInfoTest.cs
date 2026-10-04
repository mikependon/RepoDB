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
    public class UniqueConstraintInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestUniqueConstraintInfoEqualsWithSameValues()
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
        public void TestUniqueConstraintInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((UniqueConstraintInfo)null) == null);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestUniqueConstraintInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "UQ_Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoNotEqualsWithDifferentColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Columns = new List<string> { "Name" };

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoNamePropertyDefaultValue()
        {
            // Act
            var constraint = new UniqueConstraintInfo(null);
            var actual = constraint.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoNameProperty()
        {
            // Act
            var constraint = new UniqueConstraintInfo("UQ_Person_Name");
            var actual = constraint.Name;
            var expected = "UQ_Person_Name";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoColumnsPropertyDefaultValue()
        {
            // Act
            var constraint = new UniqueConstraintInfo(null);
            var actual = constraint.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestUniqueConstraintInfoColumnsProperty()
        {
            // Act
            var constraint = new UniqueConstraintInfo(null);
            constraint.Columns.Add("Id");
            constraint.Columns.Add("Name");
            var actual = constraint.Columns;
            var expected = new[] { "Id", "Name" };

            // Assert
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }

        #endregion

        #region Helpers

        private static UniqueConstraintInfo Create() =>
            new UniqueConstraintInfo("UQ_Person_Email") { Columns = new List<string> { "Email" } };

        #endregion
    }
}
