#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MySqlConnector.UnitTests
{
    [TestClass]
    public class MySqlConnectorSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlConnectorSchemaReader(null));
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaReaderCreation()
        {
            // Act
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnMySqlConnectorSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader(new MySqlConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
