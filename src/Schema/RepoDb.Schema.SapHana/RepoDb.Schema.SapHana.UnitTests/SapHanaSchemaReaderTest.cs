#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.SapHana.UnitTests
{
    [TestClass]
    public class SapHanaSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SapHanaSchemaReader(null));
        }

        [TestMethod]
        public void TestSapHanaSchemaReaderCreation()
        {
            // Act
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestSapHanaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnSapHanaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader(new HanaConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
