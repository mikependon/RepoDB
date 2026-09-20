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
    public class TestClickHouseDecimalToDecimalPropertyHandler
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
        public void TestClickHouseDecimalToDecimalPropertyHandlerSet()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToDecimalPropertyHandler();

                // Act
                var result = handler.Set(123.4567m, null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(ClickHouseDecimal));
                Assert.AreEqual(123.4567m, (decimal)(ClickHouseDecimal)result);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToDecimalPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToDecimalPropertyHandler();

                // Act
                var resultOfDecimal = handler.Get(123.4567m, null);
                var resultOfClickHouseDecimal = handler.Get(new ClickHouseDecimal(123.4567m), null);
                var resultOfInt = handler.Get(123, null);

                // Assert
                Assert.AreEqual(123.4567m, resultOfDecimal);
                Assert.AreEqual(123.4567m, resultOfClickHouseDecimal);
                Assert.AreEqual(123m, resultOfInt);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToDecimalPropertyHandlerGetWithNullValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToDecimalPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.AreEqual(0m, resultOfNull);
                Assert.AreEqual(0m, resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToDecimalPropertyHandlerInsertAndQuery()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseDecimalEntity { Id = 1, ColumnDecimal = 123.4567m };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseDecimalEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(123.4567m, result.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToDecimalPropertyHandlerInsertAndQueryWithBoundaryValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new ClickHouseDecimalEntity { Id = 1, ColumnDecimal = 0m },
                    new ClickHouseDecimalEntity { Id = 2, ColumnDecimal = -9999.9999m },
                    new ClickHouseDecimalEntity { Id = 3, ColumnDecimal = 99999999999999.9999m }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<ClickHouseDecimalEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(0m, result[0].ColumnDecimal);
                Assert.AreEqual(-9999.9999m, result[1].ColumnDecimal);
                Assert.AreEqual(99999999999999.9999m, result[2].ColumnDecimal);
            }
        }
    }
}
