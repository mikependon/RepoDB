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
    public class UniqueConstraintInfoTest
    {
        #region Methods

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
    }
}
