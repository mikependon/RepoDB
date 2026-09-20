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
    public class TestPostgreSqlTsVectorToStringPropertyHandler
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
        public void TestPostgreSqlTsVectorToStringPropertyHandlerSet()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsVectorToStringPropertyHandler();

                // Act
                var result = handler.Set("'cat':3 'fat':2 'rat':5", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsVector));
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result.ToString());
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsVectorToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsVectorToStringPropertyHandler();
                var value = NpgsqlTsVector.Parse("'cat':3 'fat':2 'rat':5");

                // Act
                var resultOfType = handler.Get(value, null);
                var resultOfString = handler.Get("'cat':3 'fat':2 'rat':5", null);

                // Assert
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", resultOfType);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", resultOfString);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsVectorToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new PostgreSqlTsVectorToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new PostgreSqlTsVectorEntity
                {
                    ColumnTsVector = "'cat':3 'fat':2 'rat':5"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<PostgreSqlTsVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result.ColumnTsVector);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new PostgreSqlTsVectorEntity
                {
                    ColumnTsVector = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<PostgreSqlTsVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsVector);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTsVectorToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new PostgreSqlTsVectorEntity { ColumnTsVector = "'cat':3 'fat':2 'rat':5" },
                    new PostgreSqlTsVectorEntity { ColumnTsVector = null },
                    new PostgreSqlTsVectorEntity { ColumnTsVector = "'cat' 'fat'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<PostgreSqlTsVectorEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result[0].ColumnTsVector);
                Assert.IsNull(result[1].ColumnTsVector);
                Assert.AreEqual("'cat' 'fat'", result[2].ColumnTsVector);
            }
        }
    }
}
