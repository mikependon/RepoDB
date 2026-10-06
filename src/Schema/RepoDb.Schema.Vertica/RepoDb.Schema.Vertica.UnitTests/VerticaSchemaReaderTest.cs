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
    [TestClass]
    public class VerticaSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaReader(null));
        }

        [TestMethod]
        public void TestVerticaSchemaReaderCreation()
        {
            // Act
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestVerticaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnVerticaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new VerticaSchemaReader(new VerticaConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
