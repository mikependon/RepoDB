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
    /// <summary>
    /// Contains the postgre sql schema reader test behavior.
    /// </summary>
    [TestClass]
    public class PostgreSqlSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on postgre sql schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new PostgreSqlSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies postgre sql schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderCreation()
        {
            // Act
            var reader = new PostgreSqlSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies postgre sql schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new PostgreSqlSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on postgre sql schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new NpgsqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on postgre sql schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new NpgsqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on postgre sql schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnPostgreSqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new NpgsqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on postgre sql schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnPostgreSqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new PostgreSqlSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new NpgsqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
