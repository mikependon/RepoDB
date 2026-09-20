#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PropertyHandlers.PostgreSql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestPostgreSqlTsQueryToStringPropertyHandler
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
        public void TestPostgreSqlTsQueryToStringPropertyHandlerSet()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set("'fat' & 'rat'", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsQuery));
                Assert.AreEqual("'fat' & 'rat'", result.ToString());
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsQueryToStringPropertyHandler();
                var value = NpgsqlTsQuery.Parse("'fat' & 'rat'");

                // Act
                var resultOfType = handler.Get(value, null);
                var resultOfString = handler.Get("'fat' & 'rat'", null);

                // Assert
                Assert.AreEqual("'fat' & 'rat'", resultOfType);
                Assert.AreEqual("'fat' & 'rat'", resultOfString);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsQueryToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsQueryToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new PostgreSqlTsQueryEntity
                {
                    ColumnTsQuery = "'fat' & 'rat'"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<PostgreSqlTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'fat' & 'rat'", result.ColumnTsQuery);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new PostgreSqlTsQueryEntity
                {
                    ColumnTsQuery = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<PostgreSqlTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsQuery);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsQueryToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new PostgreSqlTsQueryEntity { ColumnTsQuery = "'fat' & 'rat'" },
                    new PostgreSqlTsQueryEntity { ColumnTsQuery = null },
                    new PostgreSqlTsQueryEntity { ColumnTsQuery = "'cat' | 'dog'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<PostgreSqlTsQueryEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'fat' & 'rat'", result[0].ColumnTsQuery);
                Assert.IsNull(result[1].ColumnTsQuery);
                Assert.AreEqual("'cat' | 'dog'", result[2].ColumnTsQuery);
            }
        }
    }
}
