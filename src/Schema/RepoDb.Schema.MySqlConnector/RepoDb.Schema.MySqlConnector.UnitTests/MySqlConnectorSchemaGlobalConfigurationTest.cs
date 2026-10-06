#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MySqlConnector.UnitTests
{
    [TestClass]
    public class MySqlConnectorSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMySqlConnectorSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestMySqlConnectorSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlConnectorSchema();
            var actual = SchemaReaderMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
