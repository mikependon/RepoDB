#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;
using RepoDb.Schema.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyEndpointTest
    {
        [TestMethod]
        public void TestCopyEndpointDefaultValues()
        {
            // Act
            var actual = new CopyEndpoint();

            // Assert
            Assert.IsNull(actual.Server);
            Assert.IsNull(actual.Database);
            Assert.IsNull(actual.DatabaseType);
            Assert.IsNull(actual.Table);
            Assert.IsNull(actual.RowCount);
        }

        [TestMethod]
        public void TestCopyEndpointPropertiesCanBeSet()
        {
            // Setup
            var table = new TableInfo("Person", "dbo", "Source");

            // Act
            var actual = new CopyEndpoint
            {
                Server = "localhost",
                Database = "Source",
                DatabaseType = "SqlConnection",
                Table = table,
                RowCount = 25
            };

            // Assert
            Assert.AreEqual("localhost", actual.Server);
            Assert.AreEqual("Source", actual.Database);
            Assert.AreEqual("SqlConnection", actual.DatabaseType);
            Assert.AreSame(table, actual.Table);
            Assert.AreEqual(25L, actual.RowCount);
        }
    }
}
