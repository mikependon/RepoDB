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
    /// <summary>
    /// Contains the sap hana schema reader test behavior.
    /// </summary>
    [TestClass]
    public class SapHanaSchemaReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies throw exception on sap hana schema reader if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderGetTablesIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SapHanaSchemaReader().GetTables(null));
        }

        /// <summary>
        /// Verifies sap hana schema reader creation.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderCreation()
        {
            // Act
            var reader = new SapHanaSchemaReader();
            // Assert
            Assert.IsNotNull(reader);
        }

        /// <summary>
        /// Verifies sap hana schema reader is aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderIsASchemaReader()
        {
            // Act
            var reader = new SapHanaSchemaReader();
            // Assert
            Assert.IsInstanceOfType<ISchemaReader>(reader);
        }

        /// <summary>
        /// Verifies throw exception on sap hana schema reader get dependency order if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderGetDependencyOrderIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetDependencyOrder(new HanaConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sap hana schema reader table exists if the table name is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderTableExistsIfTheTableNameIsNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.TableExists(new HanaConnection(),
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sap hana schema reader get related tables if the table names are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSapHanaSchemaReaderGetRelatedTablesIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader();
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => reader.GetRelatedTables(new HanaConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on sap hana schema reader get related tables async if the table names are null.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnSapHanaSchemaReaderGetRelatedTablesAsyncIfTheTableNamesAreNull()
        {
            // Setup
            var reader = new SapHanaSchemaReader();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => reader.GetRelatedTablesAsync(new HanaConnection(),
                    null,
                    RelatedTableBehavior.ParentsAndChildren,
                    null,
                    default));
        }
        #endregion
    }
}
