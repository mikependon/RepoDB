#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaGlobalConfigurationTest
    {
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();
            var actual = SchemaComposerMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsInstanceOfType<AuroraDbPostgreSqlSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema().UseAuroraDbPostgreSqlSchema();

            // Assert
            Assert.IsInstanceOfType<AuroraDbPostgreSqlSchemaComposer>(SchemaComposerMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();

            // Assert
            Assert.IsNull(SchemaReaderMapper.Get<AuroraDbConnection>());
        }
    }
}
