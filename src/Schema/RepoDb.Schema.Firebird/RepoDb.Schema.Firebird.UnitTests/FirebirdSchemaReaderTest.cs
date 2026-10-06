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
    [TestClass]
    public class FirebirdSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FirebirdSchemaReader(null));
        }

        [TestMethod]
        public void TestFirebirdSchemaReaderCreation()
        {
            // Act
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestFirebirdSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnFirebirdSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnFirebirdSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new FirebirdSchemaReader(new FbConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
