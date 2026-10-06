#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    [TestClass]
    public class MariaDbConnectorSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MariaDbConnectorSchemaReader(null));
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderCreation()
        {
            // Act
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectorSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnMariaDbConnectorSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbConnectorSchemaReader(new MariaDbConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
