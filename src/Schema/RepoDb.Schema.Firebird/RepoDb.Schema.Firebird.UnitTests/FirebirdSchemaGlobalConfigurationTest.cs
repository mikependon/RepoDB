#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Firebird.UnitTests
{
    [TestClass]
    public class FirebirdSchemaGlobalConfigurationTest
    {
        #region Methods

        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseFirebirdSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

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

        [TestMethod]
        public void TestFirebirdSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseFirebirdSchema();
            var actual = SchemaReaderMapper.Get<FbConnection>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion
    }
}
