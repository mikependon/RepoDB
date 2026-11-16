#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.Schema.CockroachDb.UnitTests
{
    /// <summary>
    /// Contains the cockroach db schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class CockroachDbSchemaGlobalConfigurationTest
    {
        /// <summary>
        /// Verifies cockroach db schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseCockroachDbSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies cockroach db schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema();
            var actual = SchemaComposerMapper.Get<CockroachDbConnection>();
            // Assert
            Assert.IsInstanceOfType<CockroachDbSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies cockroach db schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema().UseCockroachDbSchema();
            // Assert
            Assert.IsInstanceOfType<CockroachDbSchemaComposer>(SchemaComposerMapper.Get<CockroachDbConnection>());
        }

        /// <summary>
        /// Verifies cockroach db schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema();
            // Assert
            Assert.IsNotNull(SchemaReaderMapper.Get<CockroachDbConnection>());
        }
    }
}
