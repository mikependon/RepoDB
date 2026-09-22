#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PropertyHandlers.EnterpriseDb;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.EnterpriseDb.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestTsQueryToStringPropertyHandler
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
        public void TestTsQueryToStringPropertyHandlerSet()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new TsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set("'fat' & 'rat'", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsQuery));
                Assert.AreEqual("'fat' & 'rat'", result.ToString(), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new TsQueryToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new TsQueryToStringPropertyHandler();
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
        public void TestTsQueryToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new TsQueryToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new TsQueryToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new EnterpriseDbTsQueryEntity
                {
                    ColumnTsQuery = "'fat' & 'rat'"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<EnterpriseDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'fat' & 'rat'", result.ColumnTsQuery, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new EnterpriseDbTsQueryEntity
                {
                    ColumnTsQuery = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<EnterpriseDbTsQueryEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsQuery);
            }
        }

        [TestMethod]
        public void TestTsQueryToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new EnterpriseDbTsQueryEntity { ColumnTsQuery = "'fat' & 'rat'" },
                    new EnterpriseDbTsQueryEntity { ColumnTsQuery = null },
                    new EnterpriseDbTsQueryEntity { ColumnTsQuery = "'cat' | 'dog'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<EnterpriseDbTsQueryEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'fat' & 'rat'", result[0].ColumnTsQuery, StringComparer.Ordinal);
                Assert.IsNull(result[1].ColumnTsQuery);
                Assert.AreEqual("'cat' | 'dog'", result[2].ColumnTsQuery, StringComparer.Ordinal);
            }
        }
    }
}
