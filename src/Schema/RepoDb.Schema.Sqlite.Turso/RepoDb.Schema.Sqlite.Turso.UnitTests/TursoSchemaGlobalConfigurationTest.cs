#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests
{
    /// <summary>
    /// Contains the turso schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class TursoSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies turso schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseTursoSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies turso schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var actual = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<TursoSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies turso schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var first = SchemaComposerMapper.Get<SqliteConnection>();
            GlobalConfiguration.Setup().UseTursoSchema();
            var second = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<TursoSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies turso schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseTursoSchema();
            var actual = SchemaReaderMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
