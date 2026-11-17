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
    /// <summary>
    /// Contains the aurora db my sql connector schema reader test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on aurora db my sql connector schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbMySqlConnectorSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies aurora db my sql connector schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderCreation()
        {
            // Act
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies aurora db my sql connector schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on aurora db my sql connector schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new AuroraDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db my sql connector schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new AuroraDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db my sql connector schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new AuroraDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db my sql connector schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbMySqlConnectorSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbMySqlConnectorSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new AuroraDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
