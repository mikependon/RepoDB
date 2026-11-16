#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.SapHana.UnitTests
{
    /// <summary>
    /// Contains the sap hana schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class SapHanaSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies sap hana schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseSapHanaSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies sap hana schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var actual = SchemaComposerMapper.Get<HanaConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<SapHanaSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies sap hana schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var first = SchemaComposerMapper.Get<HanaConnection>();
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var second = SchemaComposerMapper.Get<HanaConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<SapHanaSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies sap hana schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseSapHanaSchema();
            var actual = SchemaReaderMapper.Get<HanaConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
