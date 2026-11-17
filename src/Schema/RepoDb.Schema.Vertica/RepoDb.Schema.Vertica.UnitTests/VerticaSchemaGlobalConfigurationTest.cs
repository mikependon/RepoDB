#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Vertica.UnitTests
{
    /// <summary>
    /// Contains the vertica schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class VerticaSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies vertica schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseVerticaSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies vertica schema global configuration registers the schema composer.
        /// </summary>
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

        /// <summary>
        /// Verifies vertica schema global configuration can be called more than once.
        /// </summary>
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

        /// <summary>
        /// Verifies vertica schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseVerticaSchema();
            var actual = SchemaReaderMapper.Get<VerticaConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
