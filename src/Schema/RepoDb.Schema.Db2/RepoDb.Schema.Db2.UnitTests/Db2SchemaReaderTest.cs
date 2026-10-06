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
    [TestClass]
    public class Db2SchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new Db2SchemaReader(null));
        }

        [TestMethod]
        public void TestDb2SchemaReaderCreation()
        {
            // Act
            var reader = new Db2SchemaReader(new DB2Connection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestDb2SchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new Db2SchemaReader(new DB2Connection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader(new DB2Connection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new Db2SchemaReader(new DB2Connection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnDb2SchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader(new DB2Connection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnDb2SchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new Db2SchemaReader(new DB2Connection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
