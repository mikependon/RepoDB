#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Oracle.UnitTests
{
    [TestClass]
    public class OracleSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new OracleSchemaReader(null));
        }

        [TestMethod]
        public void TestOracleSchemaReaderCreation()
        {
            // Act
            var reader = new OracleSchemaReader(new OracleConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestOracleSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new OracleSchemaReader(new OracleConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader(new OracleConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new OracleSchemaReader(new OracleConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader(new OracleConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnOracleSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader(new OracleConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
