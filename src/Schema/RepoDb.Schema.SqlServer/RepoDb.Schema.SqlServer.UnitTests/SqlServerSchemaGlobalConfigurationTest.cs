#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    /// <summary>
    /// Contains the sql server schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaGlobalConfigurationTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies sql server schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseSqlServerSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies sql server schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseSqlServerSchema();
            var actual = SchemaComposerMapper.Get<SqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType<SqlServerSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies sql server schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseSqlServerSchema();
            var first = SchemaComposerMapper.Get<SqlConnection>();
            GlobalConfiguration.Setup().UseSqlServerSchema();
            var second = SchemaComposerMapper.Get<SqlConnection>();
            // Assert
            Assert.IsNotNull(second);
            Assert.IsInstanceOfType<SqlServerSchemaComposer>(second);
        }

        /// <summary>
        /// Verifies sql server schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseSqlServerSchema();
            var actual = SchemaReaderMapper.Get<SqlConnection>();
            // Assert
            Assert.IsNotNull(actual);
        }
        #endregion
    }
}
