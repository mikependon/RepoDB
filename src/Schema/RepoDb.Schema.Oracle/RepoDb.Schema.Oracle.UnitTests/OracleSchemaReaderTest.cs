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
    /// <summary>
    /// Contains the oracle schema reader test behavior.
    /// </summary>
    [TestClass]
    public class OracleSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on oracle schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new OracleSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies oracle schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaReaderCreation()
        {
            // Act
            var reader = new OracleSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies oracle schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new OracleSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on oracle schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new OracleConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on oracle schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new OracleSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new OracleConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on oracle schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnOracleSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new OracleConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on oracle schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnOracleSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new OracleSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new OracleConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
