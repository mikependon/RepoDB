#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PropertyHandlers.AuroraDb;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestAuroraDbTsQueryToStringPropertyHandler
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerSet()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new AuroraDbTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set("'fat' & 'rat'", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsQuery));
                Assert.AreEqual("'fat' & 'rat'", result.ToString(), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new AuroraDbTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new AuroraDbTsQueryToStringPropertyHandler();
                var value = NpgsqlTsQuery.Parse("'fat' & 'rat'");

                // Act
                var resultOfType = handler.Get(value, null);
                var resultOfString = handler.Get("'fat' & 'rat'", null);

                // Assert
                Assert.AreEqual("'fat' & 'rat'", resultOfType, StringComparer.Ordinal);
                Assert.AreEqual("'fat' & 'rat'", resultOfString, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new AuroraDbTsQueryToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new AuroraDbTsQueryToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new AuroraDbTsQueryEntity
                {
                    ColumnTsQuery = "'fat' & 'rat'"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<AuroraDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'fat' & 'rat'", result.ColumnTsQuery, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new AuroraDbTsQueryEntity
                {
                    ColumnTsQuery = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<AuroraDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsQuery);
            }
        }

        [TestMethod]
        public void TestAuroraDbTsQueryToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new AuroraDbTsQueryEntity { ColumnTsQuery = "'fat' & 'rat'" },
                    new AuroraDbTsQueryEntity { ColumnTsQuery = null },
                    new AuroraDbTsQueryEntity { ColumnTsQuery = "'cat' | 'dog'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<AuroraDbTsQueryEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'fat' & 'rat'", result[0].ColumnTsQuery, StringComparer.Ordinal);
                Assert.IsNull(result[1].ColumnTsQuery);
                Assert.AreEqual("'cat' | 'dog'", result[2].ColumnTsQuery, StringComparer.Ordinal);
            }
        }
    }
}
