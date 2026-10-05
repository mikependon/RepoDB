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
    [TestClass]
    public class CockroachDbSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CockroachDbSchemaReader(null));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderCreation()
        {
            // Act
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnCockroachDbSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new CockroachDbSchemaReader(new CockroachDbConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
