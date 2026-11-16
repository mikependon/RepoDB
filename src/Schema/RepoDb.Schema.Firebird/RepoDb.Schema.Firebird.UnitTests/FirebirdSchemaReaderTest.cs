#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Firebird.UnitTests
{
    /// <summary>
    /// Contains the firebird schema reader test behavior.
    /// </summary>
    [TestClass]
    public class FirebirdSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on firebird schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FirebirdSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies firebird schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderCreation()
        {
            // Act
            var reader = new FirebirdSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies firebird schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new FirebirdSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on firebird schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new FbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on firebird schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new FbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on firebird schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new FbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on firebird schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnFirebirdSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new FbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
