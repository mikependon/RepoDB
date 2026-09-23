#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.DuckDb;
using System;
using System.Linq;

namespace RepoDb.DuckDb.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestDuckDbPropertyHandlers
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

        #region DuckDbTimeOnlyToTimeSpanPropertyHandler

        [TestMethod]
        public void TestDuckDbTimeOnlyToTimeSpanPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new DuckDbTimeEntity { ColumnTime = new TimeSpan(1, 2, 3) };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<DuckDbTimeEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(new TimeSpan(1, 2, 3), result.ColumnTime);
            }
        }

        [TestMethod]
        public void TestDuckDbTimeOnlyToTimeSpanPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new DuckDbTimeEntity { ColumnTime = new TimeSpan(1, 2, 3) },
                    new DuckDbTimeEntity { ColumnTime = new TimeSpan(12, 0, 0) },
                    new DuckDbTimeEntity { ColumnTime = new TimeSpan(23, 59, 59) }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<DuckDbTimeEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(new TimeSpan(1, 2, 3), result[0].ColumnTime);
                Assert.AreEqual(new TimeSpan(12, 0, 0), result[1].ColumnTime);
                Assert.AreEqual(new TimeSpan(23, 59, 59), result[2].ColumnTime);
            }
        }

        #endregion

        #region DuckDbDateOnlyToDateTimePropertyHandler

        [TestMethod]
        public void TestDuckDbDateOnlyToDateTimePropertyHandlerInsertAndQuery()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new DuckDbDateEntity { ColumnDate = new DateTime(2026, 9, 20) };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<DuckDbDateEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual(new DateTime(2026, 9, 20), result.ColumnDate);
            }
        }

        [TestMethod]
        public void TestDuckDbDateOnlyToDateTimePropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new DuckDbDateEntity { ColumnDate = new DateTime(2026, 1, 1) },
                    new DuckDbDateEntity { ColumnDate = new DateTime(2026, 6, 15) },
                    new DuckDbDateEntity { ColumnDate = new DateTime(2026, 12, 31) }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<DuckDbDateEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(new DateTime(2026, 1, 1), result[0].ColumnDate);
                Assert.AreEqual(new DateTime(2026, 6, 15), result[1].ColumnDate);
                Assert.AreEqual(new DateTime(2026, 12, 31), result[2].ColumnDate);
            }
        }

        #endregion

        #region DuckDbStreamToByteArrayPropertyHandler

        [TestMethod]
        public void TestDuckDbStreamToByteArrayPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new DuckDbBlobEntity { ColumnBlob = new byte[] { 9, 8, 7, 6 } };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<DuckDbBlobEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                CollectionAssert.AreEqual(new byte[] { 9, 8, 7, 6 }, result.ColumnBlob);
            }
        }

        [TestMethod]
        public void TestDuckDbStreamToByteArrayPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new DuckDbBlobEntity { ColumnBlob = new byte[] { 1 } },
                    new DuckDbBlobEntity { ColumnBlob = new byte[] { 2, 2 } },
                    new DuckDbBlobEntity { ColumnBlob = new byte[] { 3, 3, 3 } }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<DuckDbBlobEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEqual(new byte[] { 1 }, result[0].ColumnBlob);
                CollectionAssert.AreEqual(new byte[] { 2, 2 }, result[1].ColumnBlob);
                CollectionAssert.AreEqual(new byte[] { 3, 3, 3 }, result[2].ColumnBlob);
            }
        }

        #endregion
    }
}
