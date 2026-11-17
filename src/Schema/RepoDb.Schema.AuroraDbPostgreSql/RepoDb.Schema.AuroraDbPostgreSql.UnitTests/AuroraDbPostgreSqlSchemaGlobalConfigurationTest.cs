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
    /// <summary>
    /// Contains the aurora db postgre sql schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaGlobalConfigurationTest
    {
        /// <summary>
        /// Verifies aurora db postgre sql schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();
            var actual = SchemaComposerMapper.Get<AuroraDbConnection>();
            // Assert
            Assert.IsInstanceOfType<AuroraDbPostgreSqlSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema().UseAuroraDbPostgreSqlSchema();
            // Assert
            Assert.IsInstanceOfType<AuroraDbPostgreSqlSchemaComposer>(SchemaComposerMapper.Get<AuroraDbConnection>());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UseAuroraDbPostgreSqlSchema();
            // Assert
            Assert.IsNotNull(SchemaReaderMapper.Get<AuroraDbConnection>());
        }
    }
}
