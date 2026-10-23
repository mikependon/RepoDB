#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyTableTest
    {
        [TestMethod]
        public void TestCopyDataTableConstructorSetsTheDataRows()
        {
            // Setup
            var rows = new List<CopyRow> { new CopyRow(new CopyColumn("Id", typeof(int)), 1) };

            // Act
            var actual = new CopyTable(rows);

            // Assert
            Assert.AreSame(rows, actual.DataRows);
            Assert.AreEqual(1, actual.DataRows.Count);
        }

        [TestMethod]
        public void TestCopyDataTableConstructorAcceptsAnEmptyList()
        {
            // Act
            var actual = new CopyTable(new List<CopyRow>());

            // Assert
            Assert.AreEqual(0, actual.DataRows.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataTableConstructorIfTheDataRowsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CopyTable(null));
        }
    }
}
