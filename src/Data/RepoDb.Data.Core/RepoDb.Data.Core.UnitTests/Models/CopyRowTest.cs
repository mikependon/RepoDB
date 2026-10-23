#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyRowTest
    {
        [TestMethod]
        public void TestCopyDataRowConstructorSetsTheColumnAndTheValue()
        {
            // Setup
            var column = new CopyColumn("Name", typeof(string));

            // Act
            var actual = new CopyRow(column, "Michael");

            // Assert
            Assert.AreSame(column, actual.Column);
            Assert.AreEqual("Michael", actual.Value);
        }

        [TestMethod]
        public void TestCopyDataRowConstructorAcceptsANullValue()
        {
            // Act
            var actual = new CopyRow(new CopyColumn("Name", typeof(string)), null);

            // Assert
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        public void TestCopyDataRowConstructorAcceptsANullColumn()
        {
            // Act
            var actual = new CopyRow(null, 1);

            // Assert
            Assert.IsNull(actual.Column);
            Assert.AreEqual(1, actual.Value);
        }
    }
}
