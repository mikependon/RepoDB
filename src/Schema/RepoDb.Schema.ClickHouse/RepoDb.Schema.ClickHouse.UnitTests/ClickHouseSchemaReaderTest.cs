#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.ClickHouse.UnitTests
{
    [TestClass]
    public class ClickHouseSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new ClickHouseSchemaReader(null));
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderCreation()
        {
            // Act
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnClickHouseSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader(new ClickHouseConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
