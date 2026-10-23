#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class EndpointTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaEndpointTablePropertyDefaultValue()
        {
            // Act
            var endpoint = new Endpoint();
            var actual = endpoint.Table;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaEndpointTableProperty()
        {
            // Setup
            var table = new TableSchema("Person", "dbo");

            // Act
            var endpoint = new Endpoint
            {
                Table = table
            };
            var actual = endpoint.Table;

            // Assert
            Assert.AreSame(table, actual);
            Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", actual.Table.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaEndpointDatabaseProperty()
        {
            // Act
            var endpoint = new Endpoint
            {
                Database = "Source"
            };
            var actual = endpoint.Database;
            var expected = "Source";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaEndpointDatabaseTypeProperty()
        {
            // Act
            var endpoint = new Endpoint
            {
                DatabaseType = "SqlConnection"
            };
            var actual = endpoint.DatabaseType;
            var expected = "SqlConnection";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaEndpointServerProperty()
        {
            // Act
            var endpoint = new Endpoint
            {
                Server = "localhost"
            };
            var actual = endpoint.Server;
            var expected = "localhost";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemaEndpointPropertiesDefaultValue()
        {
            // Act
            var endpoint = new Endpoint();

            // Assert
            Assert.IsNull(endpoint.Database);
            Assert.IsNull(endpoint.DatabaseType);
            Assert.IsNull(endpoint.Server);
        }

        #endregion
    }
}
