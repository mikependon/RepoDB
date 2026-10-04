#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.SqlServer.UnitTests
{
    [TestClass]
    public class SqlServerSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseSqlServerSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestSqlServerSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseSqlServerSchema();
            var actual = SchemaReaderMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
