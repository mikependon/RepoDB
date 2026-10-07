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
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbMySqlConnectorSchema();
            var actual = SchemaReaderMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
