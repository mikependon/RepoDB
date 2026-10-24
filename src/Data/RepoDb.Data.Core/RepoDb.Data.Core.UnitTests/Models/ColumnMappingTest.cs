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
    public class ColumnMappingTest
    {
        [TestMethod]
        public void TestCopyDataColumnMappingConstructorSetsTheSourceAndTheDestinationColumns()
        {
            // Act
            var actual = new ColumnMapping("Name", "FullName");

            // Assert
            Assert.AreEqual("Name", actual.SourceColumn);
            Assert.AreEqual("FullName", actual.DestinationColumn);
        }

        [TestMethod]
        public void TestCopyDataColumnMappingConstructorAcceptsNullColumns()
        {
            // Act
            var actual = new ColumnMapping(null, null);

            // Assert
            Assert.IsNull(actual.SourceColumn);
            Assert.IsNull(actual.DestinationColumn);
        }
    }
}
