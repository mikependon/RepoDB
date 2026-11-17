#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests
{
    /// <summary>
    /// Contains the turso schema reader test behavior.
    /// </summary>
    [TestClass]
    public class TursoSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on turso schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new TursoSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies turso schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderCreation()
        {
            // Act
            var reader = new TursoSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies turso schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new TursoSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on turso schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on turso schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new TursoSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on turso schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on turso schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnTursoSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
