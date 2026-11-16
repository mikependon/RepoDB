#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Firebird.UnitTests
{
    /// <summary>
    /// Contains the firebird schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class FirebirdSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies firebird schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseFirebirdSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies firebird schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseFirebirdSchema();
            var actual = SchemaComposerMapper.Get<FbConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<FirebirdSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies firebird schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseFirebirdSchema();
            var first = SchemaComposerMapper.Get<FbConnection>();
            GlobalConfiguration.Setup().UseFirebirdSchema();
            var second = SchemaComposerMapper.Get<FbConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<FirebirdSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies firebird schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseFirebirdSchema();
            var actual = SchemaReaderMapper.Get<FbConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
