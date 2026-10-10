#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyDataColumnTest
    {
        [TestMethod]
        public void TestCopyDataColumnConstructorSetsTheNameAndTheType()
        {
            // Act
            var actual = new CopyDataColumn("Id", typeof(int));

            // Assert
            Assert.AreEqual("Id", actual.Name);
            Assert.AreEqual(typeof(int), actual.Type);
        }

        [TestMethod]
        public void TestCopyDataColumnConstructorAcceptsANullNameAndANullType()
        {
            // Act
            var actual = new CopyDataColumn(null, null);

            // Assert
            Assert.IsNull(actual.Name);
            Assert.IsNull(actual.Type);
        }

        [TestMethod]
        public void TestCopyDataColumnKeepsTheTypeOfTheColumn()
        {
            // Act
            var actual = new CopyDataColumn("Birthday", typeof(DateTime?));

            // Assert
            Assert.AreEqual(typeof(DateTime?), actual.Type);
        }
    }
}
