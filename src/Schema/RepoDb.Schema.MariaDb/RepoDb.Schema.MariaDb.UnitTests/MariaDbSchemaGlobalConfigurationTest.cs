#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    [TestClass]
    public class MariaDbSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMariaDbSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestMariaDbSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMariaDbSchema();
            var actual = SchemaReaderMapper.Get<MariaDbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
