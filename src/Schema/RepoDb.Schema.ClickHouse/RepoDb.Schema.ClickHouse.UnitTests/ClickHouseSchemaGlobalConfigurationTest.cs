#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.ClickHouse.UnitTests
{
    /// <summary>
    /// Contains the click house schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies click house schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseClickHouseSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies click house schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseClickHouseSchema();
            var actual = SchemaComposerMapper.Get<ClickHouseConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<ClickHouseSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies click house schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseClickHouseSchema();
            var first = SchemaComposerMapper.Get<ClickHouseConnection>();
            GlobalConfiguration.Setup().UseClickHouseSchema();
            var second = SchemaComposerMapper.Get<ClickHouseConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<ClickHouseSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies click house schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseClickHouseSchema();
            var actual = SchemaReaderMapper.Get<ClickHouseConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
