#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MySql.UnitTests
{
    /// <summary>
    /// Contains the my sql schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class MySqlSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies my sql schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMySqlSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies my sql schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlSchema();
            var actual = SchemaComposerMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<MySqlSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies my sql schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlSchema();
            var first = SchemaComposerMapper.Get<MySqlConnection>();
            GlobalConfiguration.Setup().UseMySqlSchema();
            var second = SchemaComposerMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<MySqlSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies my sql schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlSchema();
            var actual = SchemaReaderMapper.Get<MySqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
