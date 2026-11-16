#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests
{
    /// <summary>
    /// Contains the ahtola schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class AhtolaSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies ahtola schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAhtolaSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies ahtola schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var actual = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<AhtolaSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies ahtola schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var first = SchemaComposerMapper.Get<SqliteConnection>();
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var second = SchemaComposerMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<AhtolaSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies ahtola schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAhtolaSchema();
            var actual = SchemaReaderMapper.Get<SqliteConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
