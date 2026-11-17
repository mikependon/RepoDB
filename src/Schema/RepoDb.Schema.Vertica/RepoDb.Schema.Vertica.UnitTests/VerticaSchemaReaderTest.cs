#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Vertica.UnitTests
{
    /// <summary>
    /// Contains the vertica schema reader test behavior.
    /// </summary>
    [TestClass]
    public class VerticaSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on vertica schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies vertica schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderCreation()
        {
            // Act
            var reader = new VerticaSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies vertica schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new VerticaSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on vertica schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new VerticaConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on vertica schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new VerticaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new VerticaConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on vertica schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new VerticaConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on vertica schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnVerticaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new VerticaConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
