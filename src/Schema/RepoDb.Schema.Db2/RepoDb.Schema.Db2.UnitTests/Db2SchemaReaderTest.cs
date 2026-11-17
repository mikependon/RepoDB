#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Db2.UnitTests
{
    /// <summary>
    /// Contains the db2 schema reader test behavior.
    /// </summary>
    [TestClass]
    public class Db2SchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on db2 schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new Db2SchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies db2 schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaReaderCreation()
        {
            // Act
            var reader = new Db2SchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies db2 schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestDb2SchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new Db2SchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on db2 schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new DB2Connection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on db2 schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new Db2SchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new DB2Connection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on db2 schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new DB2Connection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on db2 schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnDb2SchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new DB2Connection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
