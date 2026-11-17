#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MySqlConnector.UnitTests
{
    /// <summary>
    /// Contains the my sql connector schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class MySqlConnectorSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies my sql connector schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies my sql connector schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            var actual = SchemaComposerMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<MySqlConnectorSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies my sql connector schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            var first = SchemaComposerMapper.Get<MySqlConnection>();
            GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            var second = SchemaComposerMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<MySqlConnectorSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies my sql connector schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            var actual = SchemaReaderMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
