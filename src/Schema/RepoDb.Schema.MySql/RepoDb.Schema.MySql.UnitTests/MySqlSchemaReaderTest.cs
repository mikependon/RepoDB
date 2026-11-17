#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MySql.UnitTests
{
    /// <summary>
    /// Contains the my sql schema reader test behavior.
    /// </summary>
    [TestClass]
    public class MySqlSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on my sql schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MySqlSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies my sql schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaReaderCreation()
        {
            // Act
            var reader = new MySqlSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies my sql schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new MySqlSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on my sql schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new MySqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new MySqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new MySqlConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnMySqlSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new MySqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on my sql schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnMySqlSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new MySqlSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new MySqlConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
