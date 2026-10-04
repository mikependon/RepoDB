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
    [TestClass]
    public class SqlServerSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SqlServerSchemaReader(null));
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderCreation()
        {
            // Act
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnSqlServerSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.EndToEnd));
        }

        [TestMethod]
        public async Task ThrowExceptionOnSqlServerSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SqlServerSchemaReader(new SqlConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.EndToEnd));
        }

        #endregion
    }
}
