#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    /// <summary>
    /// Contains the maria db connector schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class MariaDbConnectorSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies maria db connector schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies maria db connector schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            var actual = SchemaComposerMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<MariaDbConnectorSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies maria db connector schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            var first = SchemaComposerMapper.Get<MariaDbConnection>();
            GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            var second = SchemaComposerMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<MariaDbConnectorSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies maria db connector schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            var actual = SchemaReaderMapper.Get<MariaDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
