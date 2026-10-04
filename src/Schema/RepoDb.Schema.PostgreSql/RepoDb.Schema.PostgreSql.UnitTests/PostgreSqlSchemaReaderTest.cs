#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.PostgreSql.UnitTests
{
    [TestClass]
    public class PostgreSqlSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new PostgreSqlSchemaReader(null));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderCreation()
        {
            // Act
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.All));
        }

        [TestMethod]
        public async Task ThrowExceptionOnPostgreSqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader(new NpgsqlConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.All));
        }

        #endregion
    }
}
