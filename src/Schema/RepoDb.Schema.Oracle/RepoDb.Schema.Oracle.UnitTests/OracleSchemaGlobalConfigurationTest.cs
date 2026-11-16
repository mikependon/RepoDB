#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Oracle.UnitTests
{
    /// <summary>
    /// Contains the oracle schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class OracleSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies oracle schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseOracleSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies oracle schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseOracleSchema();
            var actual = SchemaComposerMapper.Get<OracleConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<OracleSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies oracle schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseOracleSchema();
            var first = SchemaComposerMapper.Get<OracleConnection>();
            GlobalConfiguration.Setup().UseOracleSchema();
            var second = SchemaComposerMapper.Get<OracleConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<OracleSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies oracle schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseOracleSchema();
            var actual = SchemaReaderMapper.Get<OracleConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
