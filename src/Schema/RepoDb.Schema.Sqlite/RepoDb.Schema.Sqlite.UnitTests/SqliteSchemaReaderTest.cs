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

namespace RepoDb.Schema.Sqlite.UnitTests
{
    [TestClass]
    public class SqliteSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SqliteSchemaReader(null));
        }

        [TestMethod]
        public void TestSqliteSchemaReaderCreation()
        {
            // Act
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestSqliteSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnSqliteSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqliteSchemaReader(new SqliteConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
