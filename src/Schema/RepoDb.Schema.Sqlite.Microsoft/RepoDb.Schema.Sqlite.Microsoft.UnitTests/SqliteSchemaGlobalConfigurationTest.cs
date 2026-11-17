#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Sqlite.Microsoft.UnitTests
{
    /// <summary>
    /// Contains the sqlite schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class SqliteSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies sqlite schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseSqliteSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies sqlite schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseSqliteSchema();
            var actual = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<SqliteSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies sqlite schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseSqliteSchema();
            var first = SchemaComposerMapper.Get<SqliteConnection>();
            GlobalConfiguration.Setup().UseSqliteSchema();
            var second = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<SqliteSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies sqlite schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseSqliteSchema();
            var actual = SchemaReaderMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
