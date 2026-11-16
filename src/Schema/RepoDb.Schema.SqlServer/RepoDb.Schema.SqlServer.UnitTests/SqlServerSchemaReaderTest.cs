#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    /// <summary>
    /// Contains the sql server schema reader test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on sql server schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SqlServerSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies sql server schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderCreation()
        {
            // Act
            var reader = new SqlServerSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies sql server schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SqlServerSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on sql server schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new SqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sql server schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new SqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sql server schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new SqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sql server schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnSqlServerSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new SqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
