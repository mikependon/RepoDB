#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    /// <summary>
    /// Contains the aurora db my sql connector schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies aurora db my sql connector schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies aurora db my sql connector schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            var actual = SchemaComposerMapper.Get<AuroraDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<AuroraDbMySqlConnectorSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies aurora db my sql connector schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            var first = SchemaComposerMapper.Get<AuroraDbConnection>();
            GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            var second = SchemaComposerMapper.Get<AuroraDbConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<AuroraDbMySqlConnectorSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies aurora db my sql connector schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            var actual = SchemaReaderMapper.Get<AuroraDbConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
