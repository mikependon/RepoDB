#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbMySqlConnectorSchemaReader(null));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderCreation()
        {
            // Act
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader(new AuroraDbConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
