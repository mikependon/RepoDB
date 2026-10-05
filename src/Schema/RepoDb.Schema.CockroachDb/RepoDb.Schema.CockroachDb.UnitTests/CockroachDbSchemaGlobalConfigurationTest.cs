#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.Schema.CockroachDb.UnitTests
{
    [TestClass]
    public class CockroachDbSchemaGlobalConfigurationTest
    {
        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseCockroachDbSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema();
            var actual = SchemaComposerMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsInstanceOfType<CockroachDbSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema().UseCockroachDbSchema();

            // Assert
            Assert.IsInstanceOfType<CockroachDbSchemaComposer>(SchemaComposerMapper.Get<CockroachDbConnection>());
        }

        [TestMethod]
        public void TestCockroachDbSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseCockroachDbSchema();

            // Assert
            Assert.IsNull(SchemaReaderMapper.Get<CockroachDbConnection>());
        }
    }
}
