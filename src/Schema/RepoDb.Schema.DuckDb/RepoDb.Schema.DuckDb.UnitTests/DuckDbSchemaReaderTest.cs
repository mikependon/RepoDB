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
    [TestClass]
    public class DuckDbSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new DuckDbSchemaReader(null));
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderCreation()
        {
            // Act
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnDuckDbSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnDuckDbSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new DuckDbSchemaReader(new DuckDBConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
