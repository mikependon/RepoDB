#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    [TestClass]
    public class MariaDbConnectorSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMariaDbConnectorSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestMariaDbConnectorSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbConnectorSchema();
            var actual = SchemaReaderMapper.Get<MariaDbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
