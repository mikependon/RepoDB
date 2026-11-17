#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    /// <summary>
    /// Contains the maria db schema reader test behavior.
    /// </summary>
    [TestClass]
    public class MariaDbSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on maria db schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MariaDbSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies maria db schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaReaderCreation()
        {
            // Act
            var reader = new MariaDbSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies maria db schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MariaDbSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on maria db schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new MariaDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on maria db schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MariaDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new MariaDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on maria db schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new MariaDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on maria db schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnMariaDbSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MariaDbSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new MariaDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
