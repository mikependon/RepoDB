#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Oracle.UnitTests
{
    [TestClass]
    public class OracleSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseOracleSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestOracleSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseOracleSchema();
            var actual = SchemaReaderMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
