#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace RepoDb.Schema.PostgreSql.UnitTests
{
    [TestClass]
    public class PostgreSqlSchemaGlobalConfigurationTest
    {
        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationReturnsTheGlobalConfiguration()
        {
            // Act
            var actual = GlobalConfiguration.Setup().UsePostgreSqlSchema();

            // Assert
            Assert.AreSame(GlobalConfiguration.Setup(), actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationRegistersTheSchemaComposer()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema();
            var actual = SchemaComposerMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsInstanceOfType<PostgreSqlSchemaComposer>(actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationCanBeCalledMoreThanOnce()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema().UsePostgreSqlSchema();

            // Assert
            Assert.IsInstanceOfType<PostgreSqlSchemaComposer>(SchemaComposerMapper.Get<NpgsqlConnection>());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaGlobalConfigurationDoesNotRegisterASchemaReader()
        {
            // Act
            GlobalConfiguration.Setup().UsePostgreSqlSchema();

            // Assert
            Assert.IsNull(SchemaReaderMapper.Get<NpgsqlConnection>());
        }
    }
}
