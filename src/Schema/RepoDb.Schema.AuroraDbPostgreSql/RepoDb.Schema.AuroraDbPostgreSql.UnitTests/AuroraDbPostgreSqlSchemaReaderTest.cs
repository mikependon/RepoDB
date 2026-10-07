#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaReaderTest
    {
        #region Methods

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbPostgreSqlSchemaReader(null));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderCreation()
        {
            // Act
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Assert
            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(null));
        }


        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection());

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(null, CopySchemaRelationshipBehavior.ParentsAndChildren));
        }

        #endregion
    }
}
