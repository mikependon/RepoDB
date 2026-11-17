#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.DuckDb.UnitTests
{
    /// <summary>
    /// Contains the duck db schema reader test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on duck db schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new DuckDbSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies duck db schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderCreation()
        {
            // Act
            var reader = new DuckDbSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies duck db schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new DuckDbSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on duck db schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new DuckDBConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on duck db schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new DuckDBConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on duck db schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new DuckDBConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on duck db schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnDuckDbSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new DuckDBConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
