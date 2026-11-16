#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using RepoDb.Connector.CockroachDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.CockroachDb.UnitTests
{
    /// <summary>
    /// Contains the cockroach db schema reader test behavior.
    /// </summary>
    [TestClass]
    public class CockroachDbSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on cockroach db schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CockroachDbSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies cockroach db schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderCreation()
        {
            // Act
            var reader = new CockroachDbSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies cockroach db schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new CockroachDbSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on cockroach db schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new CockroachDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on cockroach db schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new CockroachDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on cockroach db schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new CockroachDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on cockroach db schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new CockroachDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
