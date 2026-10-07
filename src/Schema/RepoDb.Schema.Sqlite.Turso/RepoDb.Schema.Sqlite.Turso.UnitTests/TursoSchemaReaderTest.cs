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
    [TestClass]
    public class TursoSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new TursoSchemaReader(null));
        }

        [TestMethod]
        public void TestTursoSchemaReaderCreation()
        {
            // Act
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestTursoSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnTursoSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnTursoSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new TursoSchemaReader(new SqliteConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
