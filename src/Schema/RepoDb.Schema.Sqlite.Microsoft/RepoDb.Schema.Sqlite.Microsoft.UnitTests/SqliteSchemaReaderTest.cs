#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.UnitTests
{
    /// <summary>
    /// Contains the sqlite schema reader test behavior.
    /// </summary>
    [TestClass]
    public class SqliteSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on sqlite schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SqliteSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies sqlite schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderCreation()
        {
            // Act
            var reader = new SqliteSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies sqlite schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SqliteSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on sqlite schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sqlite schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SqliteSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sqlite schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sqlite schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnSqliteSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
