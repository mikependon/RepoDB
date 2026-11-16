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
    /// <summary>
    /// Contains the click house schema reader test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on click house schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new ClickHouseSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies click house schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderCreation()
        {
            // Act
            var reader = new ClickHouseSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies click house schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new ClickHouseSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on click house schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new ClickHouseConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on click house schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new ClickHouseConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on click house schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new ClickHouseConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on click house schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnClickHouseSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new ClickHouseSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new ClickHouseConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
