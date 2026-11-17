#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.DuckDb.UnitTests
{
    /// <summary>
    /// Contains the duck db schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies duck db schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseDuckDbSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies duck db schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var actual = SchemaComposerMapper.Get<DuckDBConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<DuckDbSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies duck db schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var first = SchemaComposerMapper.Get<DuckDBConnection>();
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var second = SchemaComposerMapper.Get<DuckDBConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<DuckDbSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies duck db schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseDuckDbSchema();
            var actual = SchemaReaderMapper.Get<DuckDBConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
