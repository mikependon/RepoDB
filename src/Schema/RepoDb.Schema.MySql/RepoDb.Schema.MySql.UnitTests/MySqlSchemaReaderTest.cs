#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MySql.UnitTests
{
    [TestClass]
    public class MySqlSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlSchemaReader(null));
        }

        [TestMethod]
        public void TestMySqlSchemaReaderCreation()
        {
            // Act
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestMySqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnMySqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader(new MySqlConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
