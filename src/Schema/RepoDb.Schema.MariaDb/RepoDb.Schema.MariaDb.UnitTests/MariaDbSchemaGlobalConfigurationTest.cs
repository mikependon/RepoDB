#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    /// <summary>
    /// Contains the maria db schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class MariaDbSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies maria db schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMariaDbSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies maria db schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbSchema();
            var actual = SchemaComposerMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<MariaDbSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies maria db schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbSchema();
            var first = SchemaComposerMapper.Get<MariaDbConnection>();
            GlobalConfiguration.Setup().UseMariaDbSchema();
            var second = SchemaComposerMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<MariaDbSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies maria db schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbSchema();
            var actual = SchemaReaderMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
