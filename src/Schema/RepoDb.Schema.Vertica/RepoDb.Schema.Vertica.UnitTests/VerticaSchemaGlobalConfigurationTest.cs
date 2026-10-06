#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Vertica.UnitTests
{
    [TestClass]
    public class VerticaSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseVerticaSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseVerticaSchema();
            var actual = SchemaComposerMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<VerticaSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseVerticaSchema();
            var first = SchemaComposerMapper.Get<VerticaConnection>();
            GlobalConfiguration.Setup().UseVerticaSchema();
            var second = SchemaComposerMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<VerticaSchemaComposer>(second);
        }

        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseVerticaSchema();
            var actual = SchemaReaderMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
