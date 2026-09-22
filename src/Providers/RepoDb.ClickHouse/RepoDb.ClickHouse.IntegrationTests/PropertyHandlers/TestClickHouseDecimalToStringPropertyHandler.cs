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
using System.Globalization;
using System.Linq;

namespace RepoDb.ClickHouse.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestClickHouseDecimalToStringPropertyHandler
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
        public void TestClickHouseDecimalToStringPropertyHandlerSetWithText()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToStringPropertyHandler();

                // Act
                var result = handler.Set("123.4567", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(ClickHouseDecimal));
                Assert.AreEqual(123.4567m, (decimal)(ClickHouseDecimal)result);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToStringPropertyHandlerSetWithNullOrWhiteSpace()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Set(null, null);
                var resultOfEmpty = handler.Set("", null);
                var resultOfWhiteSpace = handler.Set("   ", null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfEmpty);
                Assert.IsNull(resultOfWhiteSpace);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToStringPropertyHandler();

                // Act
                var resultOfDecimal = handler.Get(123.4567m, null);
                var resultOfClickHouseDecimal = handler.Get(new ClickHouseDecimal(123.4567m), null);
                var resultOfInt = handler.Get(123, null);

                // Assert
                Assert.AreEqual(123.4567m.ToString(CultureInfo.InvariantCulture), resultOfDecimal, StringComparer.Ordinal);
                Assert.AreEqual(123.4567m.ToString(CultureInfo.InvariantCulture), resultOfClickHouseDecimal, StringComparer.Ordinal);
                Assert.AreEqual("123", resultOfInt, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new ClickHouseDecimalToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseDecimalToStringEntity { Id = 1, ColumnDecimal = "123.4567" };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseDecimalToStringEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(123.4567m, decimal.Parse(result.ColumnDecimal, CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestClickHouseDecimalToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new ClickHouseDecimalToStringEntity { Id = 1, ColumnDecimal = null };

                // Act
                connection.Insert(entity);
                var result = connection.Query<ClickHouseDecimalToStringEntity>(e => e.Id == entity.Id).First();

                // Assert
                Assert.AreEqual(entity.Id, result.Id);
                Assert.IsNull(result.ColumnDecimal);
            }
        }
    }
}
