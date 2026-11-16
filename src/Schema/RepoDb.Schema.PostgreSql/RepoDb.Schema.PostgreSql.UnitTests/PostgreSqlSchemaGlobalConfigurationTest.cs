#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace RepoDb.Schema.PostgreSql.UnitTests
{
    /// <summary>
    /// Contains the postgre sql schema global configuration test behavior.
    /// </summary>
    [TestClass]
    public class PostgreSqlSchemaGlobalConfigurationTest
    {
        /// <summary>
        /// Verifies postgre sql schema global configuration returns the global configuration.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UsePostgreSqlSchema();
            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(),
                actual);
        }

        /// <summary>
        /// Verifies postgre sql schema global configuration registers the schema composer.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema();
            var actual = SchemaComposerMapper.Get<NpgsqlConnection>();
            // Assert
            Assert.IsInstanceOfType<PostgreSqlSchemaComposer>(actual);
        }

        /// <summary>
        /// Verifies postgre sql schema global configuration can be called more than once.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema().UsePostgreSqlSchema();
            // Assert
            Assert.IsInstanceOfType<PostgreSqlSchemaComposer>(SchemaComposerMapper.Get<NpgsqlConnection>());
        }

        /// <summary>
        /// Verifies postgre sql schema global configuration registers aschema reader.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationRegistersASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema();
            // Assert
            Assert.IsNotNull(SchemaReaderMapper.Get<NpgsqlConnection>());
        }
    }
}
