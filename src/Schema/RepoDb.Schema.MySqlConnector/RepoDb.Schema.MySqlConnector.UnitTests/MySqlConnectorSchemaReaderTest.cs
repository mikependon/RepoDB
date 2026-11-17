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
    /// <summary>
    /// Contains the my sql connector schema reader test behavior.
    /// </summary>
    [TestClass]
    public class MySqlConnectorSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on my sql connector schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlConnectorSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies my sql connector schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaReaderCreation()
        {
            // Act
            var reader = new MySqlConnectorSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies my sql connector schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MySqlConnectorSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on my sql connector schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new MySqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql connector schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new MySqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql connector schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlConnectorSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new MySqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql connector schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnMySqlConnectorSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlConnectorSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new MySqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
