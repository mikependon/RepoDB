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
    /// <summary>
    /// Contains the ahtola schema reader test behavior.
    /// </summary>
    [TestClass]
    public class AhtolaSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on ahtola schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AhtolaSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies ahtola schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderCreation()
        {
            // Act
            var reader = new AhtolaSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies ahtola schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new AhtolaSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on ahtola schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on ahtola schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new SqliteConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on ahtola schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnAhtolaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on ahtola schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnAhtolaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new AhtolaSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new SqliteConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
