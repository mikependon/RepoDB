#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers.SqlServer;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestSqlServerVectorToFloatArrayPropertyHandler
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
        public void TestSqlServerVectorToFloatArrayPropertyHandlerSetWithArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SqlServerVectorToFloatArrayPropertyHandler();

                // Act
                var result = handler.Set(new[] { 1.5f, 2.5f, -3.25f }, null);

                // Assert
                Assert.AreEqual("[1.5,2.5,-3.25]", result);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerSetWithNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SqlServerVectorToFloatArrayPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerGetWithSqlVector()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SqlServerVectorToFloatArrayPropertyHandler();
                var vector = new SqlVector<float>(new float[] { 1.5f, 2.5f, 3.5f });

                // Act
                var result = handler.Get(vector, null);

                // Assert
                CollectionAssert.AreEqual(new[] { 1.5f, 2.5f, 3.5f }, result);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerGetWithNullValues()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SqlServerVectorToFloatArrayPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(System.DBNull.Value, null);
                var resultOfNullVector = handler.Get(SqlVector<float>.CreateNull(3), null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
                Assert.IsNull(resultOfNullVector);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new VectorEntity { Embedding = new[] { 1.5f, 2.5f, 3.5f } };

                // Act
                var id = connection.Insert<VectorEntity, int>(entity);
                var result = connection.Query<VectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                CollectionAssert.AreEqual(entity.Embedding, result.Embedding);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new VectorEntity { Embedding = null };

                // Act
                var id = connection.Insert<VectorEntity, int>(entity);
                var result = connection.Query<VectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.Embedding);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new VectorEntity { Embedding = new[] { 1.5f, 2.5f, 3.5f } };
                entity.Id = connection.Insert<VectorEntity, int>(entity);
                entity.Embedding = new[] { 4.5f, 5.5f, 6.5f };

                // Act
                var affectedRows = connection.Update(entity);
                var result = connection.Query<VectorEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                CollectionAssert.AreEqual(new[] { 4.5f, 5.5f, 6.5f }, result.Embedding);
            }
        }

        [TestMethod]
        public void TestSqlServerVectorToFloatArrayPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new VectorEntity { Embedding = new[] { 1.0f, 2.0f, 3.0f } },
                    new VectorEntity { Embedding = null },
                    new VectorEntity { Embedding = new[] { 0.25f, -0.5f, 8.0f } }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<VectorEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEqual(new[] { 1.0f, 2.0f, 3.0f }, result[0].Embedding);
                Assert.IsNull(result[1].Embedding);
                CollectionAssert.AreEqual(new[] { 0.25f, -0.5f, 8.0f }, result[2].Embedding);
            }
        }
    }
}
