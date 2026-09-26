#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PropertyHandlers.CockroachDb;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.CockroachDb.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestCockroachDbTsQueryToStringPropertyHandler
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
        public void TestCockroachDbTsQueryToStringPropertyHandlerSet()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new CockroachDbTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set("'fat' & 'rat'", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsQuery));
                Assert.AreEqual("'fat' & 'rat'", result.ToString(), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new CockroachDbTsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new CockroachDbTsQueryToStringPropertyHandler();
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
        public void TestCockroachDbTsQueryToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new CockroachDbTsQueryToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new CockroachDbTsQueryToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new CockroachDbTsQueryEntity
                {
                    ColumnTsQuery = "'fat' & 'rat'"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<CockroachDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'fat' & 'rat'", result.ColumnTsQuery, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new CockroachDbTsQueryEntity
                {
                    ColumnTsQuery = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<CockroachDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsQuery);
            }
        }

        [TestMethod]
        public void TestCockroachDbTsQueryToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new CockroachDbTsQueryEntity { ColumnTsQuery = "'fat' & 'rat'" },
                    new CockroachDbTsQueryEntity { ColumnTsQuery = null },
                    new CockroachDbTsQueryEntity { ColumnTsQuery = "'cat' | 'dog'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<CockroachDbTsQueryEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'fat' & 'rat'", result[0].ColumnTsQuery, StringComparer.Ordinal);
                Assert.IsNull(result[1].ColumnTsQuery);
                Assert.AreEqual("'cat' | 'dog'", result[2].ColumnTsQuery, StringComparer.Ordinal);
            }
        }
    }
}
