#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.MySql.UnitTests
{
    [TestClass]
    public class MySqlSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseMySqlSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestMySqlSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseMySqlSchema();
            var actual = SchemaReaderMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
