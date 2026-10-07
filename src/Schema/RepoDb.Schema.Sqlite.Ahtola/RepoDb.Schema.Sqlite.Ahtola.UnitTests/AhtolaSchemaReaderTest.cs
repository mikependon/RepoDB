#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests
{
    [TestClass]
    public class AhtolaSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader(null));
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderCreation()
        {
            // Act
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnAhtolaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader(new SqliteConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
