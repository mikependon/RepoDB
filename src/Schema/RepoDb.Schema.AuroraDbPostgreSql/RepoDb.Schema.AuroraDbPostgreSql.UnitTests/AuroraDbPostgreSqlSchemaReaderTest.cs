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
    /// <summary>
    /// Contains the aurora db postgre sql schema reader test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbPostgreSqlSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderCreation()
        {
            // Act
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new AuroraDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new AuroraDbConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new AuroraDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on aurora db postgre sql schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbPostgreSqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AuroraDbPostgreSqlSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new AuroraDbConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
