#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using ClickHouse.Driver.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.ClickHouse;
using System;
using System.Linq;

namespace RepoDb.ClickHouse.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestClickHouseDecimalToNullableDecimalPropertyHandler
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
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerSetWithValue()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

                // Act
                var result = handler.Set(123.4567m, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(ClickHouseDecimal));
                Assert.AreEqual(123.4567m, (decimal)(ClickHouseDecimal)result);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerSetWithNull()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerGetWithValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

                // Act
                var resultOfDecimal = handler.Get(123.4567m, null);
                var resultOfClickHouseDecimal = handler.Get(new ClickHouseDecimal(123.4567m), null);

                // Assert
                Assert.AreEqual(123.4567m, resultOfDecimal);
                Assert.AreEqual(123.4567m, resultOfClickHouseDecimal);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerGetWithNullValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToNullableDecimalPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerInsertAndQuery()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseNullableDecimalEntity { Id = 1, ColumnDecimal = 123.4567m };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseNullableDecimalEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(123.4567m, result.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToNullableDecimalPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseNullableDecimalEntity { Id = 1, ColumnDecimal = null };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseNullableDecimalEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.IsNull(result.ColumnDecimal);
            }
        }
    }
}
